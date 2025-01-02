using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class QuickstepSound : ActorSound
    {
        [SerializeField] private EventReference quickstepSound;
        [SerializeField] private EventReference quickstepVoice;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateQuickstep)
            {
                RuntimeManager.PlayOneShot(quickstepSound);
                RuntimeManager.PlayOneShot(quickstepVoice);
            }
        }
    }
}