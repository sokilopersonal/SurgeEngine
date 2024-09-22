using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    // Because StepSound class is not on the animator game object
    public class StepAnimationReference : MonoBehaviour
    {
        [SerializeField] private StepSound stepSound;

        public void Play()
        {
            stepSound.Play();
        }
    }
}