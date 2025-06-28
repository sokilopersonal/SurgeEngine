using System;
using DG.Tweening;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
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
        
        private bool CanPause => _delay <= 0f;

        private float _delay;
        private Sequence _pauseFadeTween;
        private InputDevice _device;

        [Inject] private GameSettings _gameSettings;
        [Inject] private VolumeManager _volumeManager;
        [Inject] private ActorBase _actor;

        protected override void Awake()
        {
            base.Awake();
            
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
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            
            pauseActionReference.action.Disable();
            pauseActionReference.action.performed -= OnPauseAction;
        }

        private void Update()
        {
            Utility.TickTimer(ref _delay, pauseDelay, false, true);
        }

        private void OnPauseAction(InputAction.CallbackContext obj)
        {
            var context = _actor;
            if (context != null && context.StateMachine.IsExact<FStateSpecialJump>() && context.StateMachine.GetState<FStateSpecialJump>().SpecialJumpData.type ==
                SpecialJumpType.TrickJumper) return;
            
            if (!CanPause || context.IsDead || Count > 1) return; // TODO: Fix pause deactivation when popping the second page
            
            Active = !Active;

            _device = obj.control.device;
            SetPause(Active);
        }

        protected override void OnCancelAction(InputAction.CallbackContext obj)
        {
            if (Count == 1 && Active)
            {
                SetPause(false);
            }
            else
            {
                base.OnCancelAction(obj);
            }
        }

        public void SetPause(bool isPaused)
        {
            Active = isPaused;
            _delay = pauseDelay;
            
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
            
            if (_device is Keyboard)
            {
                Cursor.visible = isPaused;
                Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            }
            
            _pauseFadeTween = DOTween.Sequence();
            _pauseFadeTween.Join(_canvasGroup.DOFade(isPaused ? 1 : 0, 1f)).SetEase(Ease.OutCubic);
            _pauseFadeTween.Join(DOTween.To(() => Time.timeScale, x => Time.timeScale = x,
                isPaused ? 0f : 1f, isPaused ? 0f : 0.25f));
            _pauseFadeTween.SetLink(gameObject);
            _pauseFadeTween.SetUpdate(true);
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