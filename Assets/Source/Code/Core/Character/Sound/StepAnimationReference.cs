using SurgeEngine.Source.Code.Gameplay.Effects;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    // Because StepSound class is not on the animator game object
    public class StepAnimationReference : MonoBehaviour
    {
        [SerializeField] private StepSound stepSound;

        public void Play()
        {
            stepSound.PlaySound();
        }
    }
}