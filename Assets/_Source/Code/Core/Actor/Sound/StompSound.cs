using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class StompSound : CharacterSound
    {
        [SerializeField] private EventReference stompLoopSound;
        [SerializeField] private EventReference stompLandSound;

        private EventInstance _stompLoopInstance;

        public override void Initialize(CharacterBase character)
        {
            base.Initialize(character);
            
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

            if (Character.StateMachine.PreviousState is FStateStomp && Character.Kinematics.CheckForGround(out _))
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