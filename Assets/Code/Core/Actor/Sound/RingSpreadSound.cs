using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class RingSpreadSound : ActorSound
    {
        [SerializeField] private EventReference ringSpreadEvent;

        protected override void SoundState(FState obj)
        {
            base.SoundState(obj);

            if (obj is FStateDamage { State: DamageState.Alive }) 
                RuntimeManager.PlayOneShot(ringSpreadEvent);
        }
    }
}