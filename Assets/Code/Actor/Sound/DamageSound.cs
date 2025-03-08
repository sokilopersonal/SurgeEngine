using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class DamageSound : ActorSound
    {
        [SerializeField] private EventReference damageVoice;

        public override void Initialize(Actor actor)
        {
            base.Initialize(actor);
        }

        protected override void SoundState(FState obj)
        {
            if (obj is FStateDamage)
            {
                Voice.Play(damageVoice);
            }
        }
    }
}