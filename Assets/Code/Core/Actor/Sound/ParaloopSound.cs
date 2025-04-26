using FMODUnity;
using SurgeEngine.Code.ActorEffects;
using UnityEngine;

namespace SurgeEngine.Code.Actor.Sound
{
    public class ParaloopSound : ActorSound
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