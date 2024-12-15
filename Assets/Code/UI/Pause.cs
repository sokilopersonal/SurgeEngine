using SurgeEngine.Code.ActorSystem;
using UnityEngine;
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
            }
            else
            {
                playerInput.actions.FindActionMap("Gameplay").Enable();
            }
            
            _uiCanvasGroup.alpha = _isPaused ? 1 : 0;
            
            Cursor.visible = _isPaused;
            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            
            Time.timeScale = _isPaused ? 0f : 1f;
        }
    }
}