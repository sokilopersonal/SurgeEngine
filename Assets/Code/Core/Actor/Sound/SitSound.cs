using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class SitSound : ActorSound
    {
        [SerializeField] private EventReference sitVoice;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSit && Actor.stateMachine.PreviousState is FStateIdle)
            {
                Voice.Play(sitVoice);
            }
        }
    }
}