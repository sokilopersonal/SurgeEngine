using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class SitSound : ActorSound
    {
        [SerializeField] private EventReference sitVoice;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSit && actor.stateMachine.PreviousState is FStateIdle)
            {
                RuntimeManager.PlayOneShot(sitVoice);
            }
        }
    }
}