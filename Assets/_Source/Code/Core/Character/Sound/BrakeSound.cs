using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class BrakeSound : CharacterSound
    {
        [SerializeField] private EventReference brakeSound;

        protected override void SoundState(FState obj)
        {
            base.SoundState(obj);
            
            if (obj is FStateBrake)
            {
                RuntimeManager.PlayOneShotAttached(brakeSound, Character.gameObject);
            }
        }
    }
}