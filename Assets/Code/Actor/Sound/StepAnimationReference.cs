using SurgeEngine.Code.ActorEffects;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    // Because StepSound class is not on the animator game object
    public class StepAnimationReference : MonoBehaviour
    {
        [SerializeField] private Step stepEffect;
        [SerializeField] private StepSound stepSound;

        public void Play()
        {
            stepEffect.PlayEffect();
            stepSound.PlaySound();
        }
    }
}