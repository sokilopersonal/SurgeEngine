using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.Sound
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