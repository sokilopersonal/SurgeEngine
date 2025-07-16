using System;
using DG.Tweening;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.GoalRing;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Tools.Managers;
using SurgeEngine.Code.UI.Pages.Baseline;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.UI
{
    public class PauseHandler : PageController
    {
        public bool Active { get; private set; }

        [Header("Input")]
        [SerializeField] private InputActionReference pauseActionReference;
        [SerializeField] private float pauseDelay = 0.2f;
        
        private bool _canPause;
        
        private Sequence _pauseFadeTween;
        private InputDevice _device;

        [Inject] private GameSettings _gameSettings;
        [Inject] private VolumeManager _volumeManager;
        [Inject] private ActorBase _actor;

        protected override void Awake()
        {
            base.Awake();

            _canPause = true;
            
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            Time.timeScale = 1f;
            
            _volumeManager.ToggleGameGroup(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            pauseActionReference.action.Enable();
            pauseActionReference.action.performed += OnPauseAction;

            ObjectEvents.OnObjectTriggered += OnObjectTriggered;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            pauseActionReference.action.Disable();
            pauseActionReference.action.performed -= OnPauseAction;
            
            ObjectEvents.OnObjectTriggered -= OnObjectTriggered;
        }

        private void Update()
        {
            if (Active)
            {
                UpdateCursorBasedOnInputDevice();
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            
            void UpdateCursorBasedOnInputDevice()
            {
                var devices = InputSystem.devices;
                foreach (var device in devices)
                {
                    if (!device.wasUpdatedThisFrame) continue;

                    switch (device)
                    {
                        case Keyboard _:
                            SetCursorVisible(true);
                            break;
                        
                        case Mouse mouse:
                            if (mouse.delta.ReadValue().magnitude > 0)
                            {
                                SetCursorVisible(true);
                            }
                            break;
                        
                        case Gamepad _:
                            SetCursorVisible(false);
                            break;
                    }
                }
            }
            
            void SetCursorVisible(bool isVisible)
            {
                Cursor.visible = isVisible;
                Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }

        private void OnPauseAction(InputAction.CallbackContext obj)
        {
            var context = _actor;
            if (context != null && context.StateMachine.IsExact<FStateSpecialJump>() && context.StateMachine.GetState<FStateSpecialJump>().SpecialJumpData.type ==
                SpecialJumpType.TrickJumper) return;
            
            if (!_canPause || context.IsDead) return;
            
            Active = !Active;

            _device = obj.control.device;
            SetPause(Active);
        }

        protected override void OnCancelAction(InputAction.CallbackContext obj)
        {
            // Double input fix
            if (obj.control.device is Keyboard)
            {
                if (Count > 1 && Active)
                {
                    _canPause = false;
                    base.OnCancelAction(obj);
                }
                else
                {
                    _canPause = true;
                }
            }
            else
            {
                if (Count == 1 && Active)
                {
                    _canPause = true;
                    SetPause(false);
                }
                else
                {
                    base.OnCancelAction(obj);
                }
            }
        }

        public void SetPause(bool isPaused)
        {
            Active = isPaused;
            
            if (Active)
            {
                _volumeManager.ToggleGameGroup(false);
                
                if (initial)
                {
                    initial.Enter();
                }
            }
            else
            {
                _volumeManager.ToggleGameGroup(true);

                if (initial)
                {
                    PopAllPages();
                    
                    initial.Exit();
                }
            }
            
            _canvasGroup.interactable = isPaused;
            
            var context = _actor;
            if (context)
            {
                PlayerInput playerInput = context.Input.playerInput;
                playerInput.enabled = !isPaused;
            }
            
            _pauseFadeTween = DOTween.Sequence();
            _pauseFadeTween.Join(_canvasGroup.DOFade(isPaused ? 1 : 0, 1f)).SetEase(Ease.OutCubic);
            _pauseFadeTween.Join(DOTween.To(() => Time.timeScale, x => Time.timeScale = x,
                isPaused ? 0f : 1f, isPaused ? 0f : 0.25f));
            _pauseFadeTween.SetLink(gameObject);
            _pauseFadeTween.SetUpdate(true);
        }

        private void OnObjectTriggered(ContactBase obj)
        {
            if (obj is GoalRing)
            {
                enabled = false;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                if (!_gameSettings.RunInBackground)
                {
                    SetPause(true);
                }
            }
        }

        public void RestartAction()
        {
            SceneLoader.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitAction()
        {
            //SceneLoader.LoadScene("Scene1");
        }
    }
}