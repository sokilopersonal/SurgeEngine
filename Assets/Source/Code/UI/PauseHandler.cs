using DG.Tweening;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.GoalRing;
using SurgeEngine.Source.Code.Infrastructure.Tools.Managers;
using SurgeEngine.Source.Code.UI.Pages.Baseline;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Source.Code.UI
{
    public class PauseHandler : PageController
    {
        public bool Active { get; private set; }

        [SerializeField] private UpdateCursorDevice updateCursorDevice;

        [Header("Input")]
        [SerializeField] private InputActionReference pauseActionReference;
        
        private Sequence _pauseFadeTween;

        [Inject] private GameSettings _gameSettings;
        [Inject] private VolumeManager _volumeManager;
        [Inject] private CharacterBase _character;

        protected override void Awake()
        {
            base.Awake();
            
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
                updateCursorDevice.UpdateDevice();
            }
        }

        private void OnPauseAction(InputAction.CallbackContext obj)
        {
            var context = _character;
            if (context != null && context.StateMachine.IsExact<FStateSpecialJump>() && context.StateMachine.GetState<FStateSpecialJump>().SpecialJumpData.type ==
                SpecialJumpType.TrickJumper) return;
            
            if (context.Life.IsDead) return;
            
            if (Count > 1 && Active && obj.control.device is Keyboard)
            {
                return;
            }
            Active = !Active;
            
            SetPause(Active);
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
                
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
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

        private void OnObjectTriggered(StageObject obj)
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

        private void OnDestroy()
        {
            _volumeManager.ToggleMenuDistortion(false);
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