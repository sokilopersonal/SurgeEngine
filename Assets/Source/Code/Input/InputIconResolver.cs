using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Source.Code.Input
{
    public class InputIconResolver
    {
        private readonly InputIconDatabase _database;

        private InputDevice _lastDevice;

        public InputIconResolver(InputIconDatabase database)
        {
            _database = database;

            InputSystem.onActionChange += OnActionChange;
        }

        ~InputIconResolver()
        {
            InputSystem.onActionChange -= OnActionChange;
        }

        private void OnActionChange(
            object obj,
            InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed)
                return;

            if (obj is InputAction action &&
                action.activeControl != null)
            {
                _lastDevice = action.activeControl.device;
            }
        }

        public Sprite GetSprite(
            InputAction action,
            int bindingIndex)
        {
            if (action == null)
                return null;

            if (bindingIndex < 0 ||
                bindingIndex >= action.bindings.Count)
            {
                return null;
            }

            return GetSprite(action.bindings[bindingIndex]);
        }

        public Sprite GetSprite(InputBinding binding)
        {
            return GetSprite(binding, GetDeviceType(binding.effectivePath));
        }

        public Sprite GetSprite(
            InputBinding binding,
            InputIconDatabase.DeviceType device)
        {
            string path = binding.effectivePath;

            if (string.IsNullOrEmpty(path))
                return null;

            return _database.GetSprite(
                path,
                device);
        }

        private InputIconDatabase.DeviceType GetDeviceType(
            string path)
        {
            if (path.StartsWith("<Keyboard>"))
                return InputIconDatabase.DeviceType.Keyboard;

            if (path.StartsWith("<Mouse>"))
                return InputIconDatabase.DeviceType.Mouse;

            if (_lastDevice != null)
            {
                string layout =
                    _lastDevice.layout.ToLowerInvariant();

                if (layout.Contains("dualshock") ||
                    layout.Contains("dualsense") ||
                    layout.Contains("playstation"))
                {
                    return InputIconDatabase.DeviceType.PlayStation;
                }

                if (layout.Contains("xinput") ||
                    layout.Contains("xbox") ||
                    layout == "gamepad")
                {
                    return InputIconDatabase.DeviceType.Xbox;
                }
            }

            return InputIconDatabase.DeviceType.Any;
        }
    }
}
