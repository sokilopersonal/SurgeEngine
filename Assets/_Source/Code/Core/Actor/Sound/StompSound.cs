using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
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

            if (Actor.stateMachine.PreviousState is FStateStomp && Actor.kinematics.CheckForGround(out _, castDistance: 1f))
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