using System;
using DG.Tweening;
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
        private float _delayTimer;
        private bool CanPause => _delayTimer <= 0f;
        private const float DelayTime = 1f;
        
        private CanvasGroup _uiCanvasGroup;
        private PlayerInput _uiInput;
        private InputAction _pauseInputAction;

        private bool _isPaused;
        private Sequence _pauseFadeTween;
        private Tween _timeFadeTween;

        private void Awake()
        {
            _uiCanvasGroup = GetComponent<CanvasGroup>();
            _uiInput = GetComponent<PlayerInput>();

            _uiCanvasGroup.alpha = 0f;
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

        private void Update()
        {
            Common.TickTimer(ref _delayTimer, DelayTime, false, true);
        }

        private void OnPauseAction(InputAction.CallbackContext obj)
        {
            _isPaused = !_isPaused;
                
            SetPause(_isPaused);
        }

        public void SetPause(bool isPaused)
        {
            if (_pauseFadeTween != null && _pauseFadeTween.IsPlaying()) return;
    
            _delayTimer = DelayTime;
            _uiCanvasGroup.interactable = isPaused;
            
            _pauseFadeTween = DOTween.Sequence();
            _pauseFadeTween.Append(_uiCanvasGroup.DOFade(isPaused ? 1 : 0, 0.4f).SetUpdate(true));
            _pauseFadeTween.Join(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 
                isPaused ? 0f : 1f, isPaused ? 0f : 0.25f).SetUpdate(true)).SetUpdate(true);
            _pauseFadeTween.SetLink(gameObject);
    
            var context = ActorContext.Context;
            if (context)
            {
                PlayerInput playerInput = context.input.playerInput;
                playerInput.enabled = !isPaused;
            }
    
            Cursor.visible = isPaused;
            Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void RestartAction()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}