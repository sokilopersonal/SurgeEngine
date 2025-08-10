using FMODUnity;
using SurgeEngine.Code.Gameplay.Effects;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class ParaloopSound : CharacterSound
    {
        [SerializeField] private EventReference paraloopSound;
        [SerializeField] private ParaloopEffect paraloopEffect;

        protected override void OnEnable()
        {
            base.OnEnable();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public void Play()
        {
            RuntimeManager.PlayOneShotAttached(paraloopSound, gameObject);
        }
    }
}