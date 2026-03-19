using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.Inputs
{
    public sealed class Rumble : ITickable, ILateDisposable
    {
        private static Rumble _instance;
        private float _timer;
        private InputDevice _device;

        [Inject] private void Inject(Rumble self) => _instance = self;

        public static void Vibrate(float low, float high, float duration = 0.2f)
        {
            Gamepad pad = Gamepad.current;
            GameDevice device = _instance.GetDevice();
            
            if (pad == null || device == GameDevice.Keyboard)
                return;

            _instance.Rumbling(low, high, duration);
        }

        private void Rumbling(float low, float high, float duration)
        {
            _timer = duration;

            Gamepad.current.SetMotorSpeeds(low, high);
        }

        public void Tick()
        {
            UpdateDevice();
            
            Gamepad pad = Gamepad.current;
            if (pad == null)
                return;
            
            if (_timer > 0)
            {
                _timer -= Time.unscaledDeltaTime;
            }
            else
            {
                pad.SetMotorSpeeds(0, 0);
            }
        }

        private void UpdateDevice()
        {
            ReadOnlyArray<InputDevice> devices = InputSystem.devices;
            foreach (InputDevice device in devices)
            {
                if (device.wasUpdatedThisFrame)
                {
                    if (device is Keyboard)
                    {
                        _device = device;
                    }
                    else if (device is Gamepad)
                    {
                        _device = device;
                    }
                }
            }
        }
        
        public GameDevice GetDevice()
        {
            GameDevice device = GameDevice.Keyboard;

            switch (_device)
            {
                case Keyboard:
                    device = GameDevice.Keyboard;
                    break;
                case XInputController:
                    device = GameDevice.XboxController;
                    break;
                case Gamepad:
                {
                    if (_device is DualShockGamepad)
                    {
                        device = GameDevice.Playstation;
                    }

                    break;
                }
            }

            return device;
        }

        public void LateDispose()
        {
            Gamepad.current?.SetMotorSpeeds(0, 0);
        }
    }
}