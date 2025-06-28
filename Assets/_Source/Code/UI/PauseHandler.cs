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
    public class PauseHandler : MonoBehaviour
    {
        public bool Active { get; private set; }
        public event Action<bool> OnPauseChanged; 

        [Header("Input")]
        [SerializeField] private InputActionReference pauseInputReference;
        [SerializeField] private float pauseDelay = 0.5f;

        [Header("Pause Page")] 
        [SerializeField] private Page pausePage;
        
        private bool CanPause => _delay <= 0f;

        private float _delay;
        private CanvasGroup _uiCanvasGroup;
        private Sequence _pauseFadeTween;
        private InputAction _pauseAction;
        private InputDevice _device;
        private PageController _pageController;

        [Inject] private GameSettings _gameSettings;
        [Inject] private VolumeManager _volumeManager;
        [Inject] private ActorBase _actor;
        
        private void Awake()
        {
            _uiCanvasGroup = GetComponent<CanvasGroup>();
            
            TryGetComponent(out _pageController);
            
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
            var context = _actor;
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
            
            if (Active)
            {
                _volumeManager.ToggleGameGroup(false);
                
                if (_pageController && pausePage)
                {
                    _pageController.PopAllPages();
                }
            }
            else
            {
                _volumeManager.ToggleGameGroup(true);
            }
            
            _uiCanvasGroup.interactable = isPaused;
            
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
            
            _pauseFadeTween?.Kill(true);
            _pauseFadeTween = DOTween.Sequence();
            _pauseFadeTween.Append(_uiCanvasGroup.DOFade(isPaused ? 1 : 0, 0.75f)).SetEase(Ease.OutCubic);
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