using FMODUnity;
using SurgeEngine.Code.ActorEffects;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
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