using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class QuickstepSound : CharacterSound
    {
        [SerializeField] private EventReference quickstepSound;
        [SerializeField] private EventReference quickstepVoice;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateQuickstep)
            {
                RuntimeManager.PlayOneShot(quickstepSound);
                Voice.Play(quickstepVoice);
            }
        }
    }
}