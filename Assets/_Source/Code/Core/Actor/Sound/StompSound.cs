using System;
using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.Core.Actor.Sound
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
            RuntimeManager.AttachInstanceToGameObject(_stompLoopInstance, transform);
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

        private void OnDestroy()
        {
            _stompLoopInstance.stop(STOP_MODE.IMMEDIATE);
            _stompLoopInstance.release();
        }
    }
}