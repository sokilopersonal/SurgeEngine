using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class BrakeSound : ActorSound
    {
        [SerializeField] private EventReference brakeSound;

        protected override void SoundState(FState obj)
        {
            base.SoundState(obj);
            
            if (obj is FStateBrake)
            {
                RuntimeManager.PlayOneShotAttached(brakeSound, actor.gameObject);
            }
        }
    }
}