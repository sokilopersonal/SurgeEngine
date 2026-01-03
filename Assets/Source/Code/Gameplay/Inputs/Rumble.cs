using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.Inputs
{
    public sealed class Rumble : ITickable, ILateDisposable
    {
        private static Rumble instance;
        private float _timer;

        [Inject] private void Inject(Rumble self) => instance = self;

        public static void Vibrate(float low, float high, float duration = 0.2f)
        {
            Gamepad pad = Gamepad.current;
            GameDevice device = CharacterContext.Context.Input.GetDevice();
            
            if (pad == null || device == GameDevice.Keyboard)
                return;

            instance.Rumbling(low, high, duration);
        }

        private void Rumbling(float low, float high, float duration)
        {
            _timer = duration;

            Gamepad.current.SetMotorSpeeds(low, high);
        }

        public void Tick()
        {
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

        public void LateDispose()
        {
            Gamepad.current?.SetMotorSpeeds(0, 0);
        }
    }
}