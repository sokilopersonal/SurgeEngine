using FMODUnity;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Actor.Sound
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