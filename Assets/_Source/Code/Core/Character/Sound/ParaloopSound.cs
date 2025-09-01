using FMODUnity;
using SurgeEngine._Source.Code.Gameplay.Effects;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.Sound
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