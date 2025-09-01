using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.Sound
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