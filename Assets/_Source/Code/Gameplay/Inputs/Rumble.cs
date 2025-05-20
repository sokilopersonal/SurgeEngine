using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.Gameplay.Inputs
{
    public class Rumble
    {
        public void Vibrate(float low, float high, float duration)
        {
            if (Gamepad.current == null)
                return;

            if (Gamepad.current.wasUpdatedThisFrame) // We don't want to rumble if the input was from another device
            {
                _ = Rumbling(low, high, duration);
            }
        }

        private async UniTask Rumbling(float low, float high, float duration)
        {
            float t = 0;
            Gamepad pad = Gamepad.current;
            pad.SetMotorSpeeds(low, high);
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }
            
            pad.SetMotorSpeeds(0, 0);
        }
    }
}