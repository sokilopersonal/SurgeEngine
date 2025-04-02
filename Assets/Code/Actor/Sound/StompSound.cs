using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.Actor.Sound
{
    public class StompSound : ActorSound
    {
        [SerializeField] private EventReference stompLoopSound;
        [SerializeField] private EventReference stompLandSound;

        private EventInstance _stompLoopInstance;

        public override void Initialize(ActorBase actor)
        {
            base.Initialize(actor);
            
            _stompLoopInstance = RuntimeManager.CreateInstance(stompLoopSound);
        }

        protected override void SoundState(FState obj)
        {
            if (obj is FStateStomp)
            {
                _stompLoopInstance.start();
            }
            else
            {
                _stompLoopInstance.stop(STOP_MODE.IMMEDIATE);
            }

            if (obj is FStateGround or FStateSpecialJump or FStateStompLand or FStateSlide && Actor.stateMachine.PreviousState is FStateStomp && Common.CheckForGround(out RaycastHit hit))
            {
                RuntimeManager.PlayOneShot(stompLandSound);
            }
        }
    }
}