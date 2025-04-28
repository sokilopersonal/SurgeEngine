using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class DamageSound : ActorSound
    {
        [SerializeField] private EventReference damageVoice;
        [SerializeField] private EventReference deathSound;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateDamage dmg)
            {
                if (dmg.State == DamageState.Alive) Voice.Play(damageVoice, true);
                else
                {
                    Voice.Play(deathSound, true);
                }
            }
        }
    }
}