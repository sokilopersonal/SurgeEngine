using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class StepAnimationReference : MonoBehaviour
    {
        [SerializeField] private StepSound stepSound;

        public void Play()
        {
            stepSound.Play();
        }
    }
}