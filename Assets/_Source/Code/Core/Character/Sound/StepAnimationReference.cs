using SurgeEngine._Source.Code.Gameplay.Effects;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.Sound
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