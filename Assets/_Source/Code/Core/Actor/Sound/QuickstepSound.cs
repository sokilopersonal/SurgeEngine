using FMODUnity;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
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