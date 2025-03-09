using FMODUnity;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Actor.Sound
{
    public class BrakeSound : ActorSound
    {
        [SerializeField] private EventReference brakeSound;

        protected override void SoundState(FState obj)
        {
            base.SoundState(obj);
            
            if (obj is FStateBrake)
            {
                RuntimeManager.PlayOneShotAttached(brakeSound, Actor.gameObject);
            }
        }
    }
}