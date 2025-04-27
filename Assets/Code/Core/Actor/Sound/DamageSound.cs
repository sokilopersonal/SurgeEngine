using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
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