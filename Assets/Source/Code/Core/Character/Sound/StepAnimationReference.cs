using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    // Because StepSound class is not on the animator game object
    public class StepAnimationReference : MonoBehaviour
    {
        [SerializeField] private StepSound stepSound;

        private float _timer;

        private void Update()
        {
            if (_timer > 0f)
                _timer -= Time.deltaTime;
        }

        public void Play()
        {
            if (_timer <= 0)
            {
                stepSound.PlaySound();
                _timer = 0.04f;
            }
        }
    }
}