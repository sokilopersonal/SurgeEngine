using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.UI.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.UI
{
    public class Pause : MonoBehaviour
    {
        private CanvasGroup _uiCanvasGroup;
        
        private PlayerInput _uiInput;
        private InputAction _pauseInputAction;

        private bool _isPaused;

        private void Awake()
        {
            _uiCanvasGroup = GetComponent<CanvasGroup>();
            _uiInput = GetComponent<PlayerInput>();

            _uiCanvasGroup.alpha = 0f;
            
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
            _isPaused = !_isPaused;
            var playerInput = ActorContext.Context.input.playerInput;
            if (_isPaused)
            {
                playerInput.actions.FindActionMap("Gameplay").Disable();
                EventSystem.current.SetSelectedGameObject(GetComponentInChildren<SettingsBarElement>().gameObject);
            }
            else
            {
                playerInput.actions.FindActionMap("Gameplay").Enable();
                EventSystem.current.SetSelectedGameObject(null);
            }
            
            _uiCanvasGroup.alpha = _isPaused ? 1 : 0;
            
            Cursor.visible = _isPaused;
            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            
            Time.timeScale = _isPaused ? 0f : 1f;
        }
    }
}