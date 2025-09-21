using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine._Source.Code.UI
{
    public class UpdateCursorDevice : MonoBehaviour
    {
        [SerializeField] private bool auto = true;
        
        private void Update()
        {
            if (auto)
            {
                UpdateDevice();
            }
        }

        public void UpdateDevice()
        {
            var devices = InputSystem.devices;
            foreach (var device in devices)
            {
                if (!device.wasUpdatedThisFrame) continue;

                switch (device)
                {
                    case Keyboard:
                        SetCursorVisible(true);
                        break;
                        
                    case Mouse mouse:
                        if (mouse.delta.ReadValue().magnitude > 0.1f)
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

        private void SetCursorVisible(bool isVisible)
        {
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}