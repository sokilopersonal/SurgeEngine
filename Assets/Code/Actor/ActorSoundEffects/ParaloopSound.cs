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
            
            paraloopEffect.OnParaloopToggle += Play;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            
            paraloopEffect.OnParaloopToggle -= Play;
        }

        public void Play(bool value)
        {
            // Paraloop effect should not depend on sound class, so handle the paraloop sound using an action
            if (value) RuntimeManager.PlayOneShotAttached(paraloopSound, gameObject);
        }
    }
}