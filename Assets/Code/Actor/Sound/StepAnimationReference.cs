using SurgeEngine.Code.ActorEffects;
using SurgeEngine.Code.Effects;
using UnityEngine;

namespace SurgeEngine.Code.Actor.Sound
{
    // Because StepSound class is not on the animator game object
    public class StepAnimationReference : MonoBehaviour
    {
        [SerializeField] private Step stepEffect;
        [SerializeField] private StepSound stepSound;

        public void Play()
        {
            stepSound.PlaySound();
        }
    }
}