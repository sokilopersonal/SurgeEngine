using System;
using DG.Tweening;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.UI
{
    public class PauseHandler : MonoBehaviour
    {
        public bool Active { get; private set; }
        public event Action<bool> OnPauseChanged; 

        [SerializeField] private InputActionReference pauseInputReference;
        [SerializeField] private float pauseDelay = 0.5f;
        
        private bool CanPause => _delay <= 0f;

        private float _delay;
        private CanvasGroup _uiCanvasGroup;
        private Sequence _pauseFadeTween;
        private InputAction _pauseAction;
        private InputDevice _device;

        [Inject] private GameSettings _gameSettings;
        [Inject] private VolumeManager _volumeManager;
        
        private void Awake()
        {
            _uiCanvasGroup = GetComponent<CanvasGroup>();

            _uiCanvasGroup.alpha = 0f;
            _uiCanvasGroup.interactable = false;
            Time.timeScale = 1f;
            
            _pauseAction = pauseInputReference.action;
            _pauseAction.Enable();
            
            _volumeManager.ToggleGameGroup(true);
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            _pauseAction.ApplyBindingOverride("<Keyboard>/tab", path: "<Keyboard>/escape");
#endif
            
            _pauseAction.performed += OnPauseAction;
        }

        private void OnDisable()
        {
            _pauseAction.performed -= OnPauseAction;
        }

        private void Update()
        {
            Utility.TickTimer(ref _delay, pauseDelay, false, true);
        }

        private void OnPauseAction(InputAction.CallbackContext obj)
        {
            var context = ActorContext.Context;
            if (context != null && context.StateMachine.IsExact<FStateSpecialJump>() && context.StateMachine.GetState<FStateSpecialJump>().SpecialJumpData.type ==
                SpecialJumpType.TrickJumper) return;
            
            if (!CanPause || context.IsDead) return;
            
            Active = !Active;
            OnPauseChanged?.Invoke(Active);

            _device = obj.control.device;
            SetPause(Active);
        }

        public void SetPause(bool isPaused)
        {
            Active = isPaused;
            _delay = pauseDelay;
            
            // if (Active)
            // {
            //     MenusHandler.Instance.OpenMenu<Pause>();
            //     _volumeManager.ToggleGameGroup(false);
            // }
            // else
            // {
            //     MenusHandler.Instance.CloseMenu<Pause>();
            //     _volumeManager.ToggleGameGroup(true);
            // }
            
            _uiCanvasGroup.interactable = isPaused;
            
            var context = ActorContext.Context;
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
            
            _pauseFadeTween?.Kill(true);
            _pauseFadeTween = DOTween.Sequence();
            _pauseFadeTween.Append(_uiCanvasGroup.DOFade(isPaused ? 1 : 0, 0.4f));
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