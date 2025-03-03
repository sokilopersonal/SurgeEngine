using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class HomingSound : ActorSound
    {
        [SerializeField] private EventReference homingSound;
        
        protected override void SoundState(FState obj)
        {
            if (obj is FStateHoming)
            {
                RuntimeManager.PlayOneShot(homingSound);
            }
        }
    }
}