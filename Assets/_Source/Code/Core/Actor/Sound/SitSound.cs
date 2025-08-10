using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class SitSound : CharacterSound
    {
        [SerializeField] private EventReference sitVoice;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSit && Character.StateMachine.PreviousState is FStateIdle)
            {
                Voice.Play(sitVoice);
            }
        }
    }
}