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
        
        public static void Vibrate(float low, float high, float duration)
        {
            var pad = Gamepad.current;
            if (pad == null)
                return;

            if (pad.wasUpdatedThisFrame) // We don't want to rumble if the input was from another device
            {
                instance.Rumbling(low, high, duration);
            }
        }

        private void Rumbling(float low, float high, float duration = 0.2f)
        {
            _timer = duration;
            
            Gamepad.current.SetMotorSpeeds(low, high);
        }

        public void Tick()
        {
            var pad = Gamepad.current;
            if (pad != null)
            {
                if (_timer > 0)
                {
                    _timer -= Time.unscaledDeltaTime;
                }
                else
                {
                    pad.SetMotorSpeeds(0, 0);
                }
            }
        }

        public void LateDispose()
        {
            Gamepad.current?.SetMotorSpeeds(0, 0);
        }
    }
}