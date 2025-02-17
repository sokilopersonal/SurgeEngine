using System;
using DG.Tweening;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.UI.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SurgeEngine.Code.UI
{
    public class Pause : MonoBehaviour
    {
        public static Pause Instance { get; private set; }
        public bool Active { get; private set; }
        
        private bool CanPause => _pauseFadeTween == null;
        
        private CanvasGroup _uiCanvasGroup;
        private PlayerInput _uiInput;
        private InputAction _pauseInputAction;

        private Sequence _pauseFadeTween;
        private Tween _timeFadeTween;

        private void Awake()
        {
            Instance = this;
            
            _uiCanvasGroup = GetComponent<CanvasGroup>();
            _uiInput = GetComponent<PlayerInput>();

            _uiCanvasGroup.alpha = 0f;
            _uiCanvasGroup.interactable = false;
            Time.timeScale = 1f;
            
            _pauseInputAction = _uiInput.actions["Pause"];
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            _pauseInputAction.ApplyBindingOverride("<Keyboard>/tab", path: "<Keyboard>/escape");
#endif
            
            _pauseInputAction.performed += OnPauseAction;
        }

        private void OnDisable()
        {
            _pauseInputAction.performed -= OnPauseAction;
        }

        private void OnPauseAction(InputAction.CallbackContext obj)
        {
            var context = ActorContext.Context;
            if (context.stateMachine.IsExact<FStateSpecialJump>() && context.stateMachine.GetState<FStateSpecialJump>().data.type ==
                SpecialJumpType.TrickJumper) return;
            
            Active = !Active;
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            
            SetPause(Active);
        }

        public async void SetPause(bool isPaused)
        {
            if (!CanPause) return;
            
            _uiCanvasGroup.interactable = isPaused;
            
            var context = ActorContext.Context;
            if (context)
            {
                PlayerInput playerInput = context.input.playerInput;
                playerInput.enabled = !isPaused;
            }
    
            Cursor.visible = isPaused;
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            
            _pauseFadeTween = DOTween.Sequence();
            _pauseFadeTween.Append(_uiCanvasGroup.DOFade(isPaused ? 1 : 0, 0.4f).SetUpdate(true));
            _pauseFadeTween.Join(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 
                isPaused ? 0f : 1f, isPaused ? 0f : 0.25f).SetUpdate(true)).SetUpdate(true);
            _pauseFadeTween.SetLink(gameObject);
            await _pauseFadeTween.AsyncWaitForCompletion();
            
            _pauseFadeTween?.Kill();
            _pauseFadeTween = null;
        }

        public void RestartAction()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}