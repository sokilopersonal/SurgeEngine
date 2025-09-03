using DG.Tweening;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.GoalRing;
using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using SurgeEngine._Source.Code.UI.Pages.Baseline;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine._Source.Code.UI
{
    public class PauseHandler : PageController
    {
        public bool Active { get; private set; }

        [Header("Input")]
        [SerializeField] private InputActionReference pauseActionReference;
        
        private bool _canPause;
        
        private Sequence _pauseFadeTween;

        [Inject] private GameSettings _gameSettings;
        [Inject] private VolumeManager _volumeManager;
        [Inject] private CharacterBase _character;

        protected override void Awake()
        {
            base.Awake();

            _canPause = true;
            
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            Time.timeScale = 1f;
        }

        protected override void OnEnable()
        {
#if UNITY_EDITOR
            pauseActionReference.action.ApplyBindingOverride("<Keyboard>/tab", null, "<Keyboard>/escape");
#endif
            
            pauseActionReference.action.Enable();
            pauseActionReference.action.performed += OnPauseAction;
            
            base.OnEnable();

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
            var context = _character;
            if (context != null && context.StateMachine.IsExact<FStateSpecialJump>() && context.StateMachine.GetState<FStateSpecialJump>().SpecialJumpData.type ==
                SpecialJumpType.TrickJumper) return;
            
            if (context.Life.IsDead) return;
            
            if (Count > 1 && Active)
            {
                return;
            }
            Active = !Active;
            
            SetPause(Active);
        }

        protected override void OnCancelAction(InputAction.CallbackContext obj)
        {
            base.OnCancelAction(obj);
        }

        public void SetPause(bool isPaused)
        {
            Active = isPaused;
            
            if (Active)
            {
                _volumeManager.ToggleMenuDistortion(true);
                
                if (initial)
                {
                    initial.Enter();
                }
            }
            else
            {
                _volumeManager.ToggleMenuDistortion(false);
                
                if (initial)
                {
                    PopAllPages();
                    
                    initial.Exit();
                }
            }
            
            _canvasGroup.interactable = isPaused;
            
            var context = _character;
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
                    if (!Active)
                    {
                        SetPause(true);
                    }
                }
            }
        }

        public void RestartAction()
        {
            SceneLoader.LoadGameScene(SceneManager.GetActiveScene().name);
        }

        public void QuitAction()
        {
            SceneLoader.LoadGameScene("MainMenu");
        }
    }
}