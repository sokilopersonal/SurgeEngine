using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace SurgeEngine.Code.UI
{
    public class UIInput : MonoBehaviour
    {
        private InputSystemUIInputModule _module;

        private void Awake()
        {
            _module = GetComponent<InputSystemUIInputModule>();
        }

        private void OnEnable()
        {
            _module.move.action.performed += OnMove;
        }

        private void OnDisable()
        {
            _module.move.action.performed -= OnMove;
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem.currentSelectedGameObject == null && eventSystem.firstSelectedGameObject.activeInHierarchy)
            {
                eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
            }
        }
    }
}