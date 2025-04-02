using FMODUnity;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Actor.Sound
{
    public class DamageSound : ActorSound
    {
        [SerializeField] private EventReference damageVoice;

        public override void Initialize(ActorBase actor)
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