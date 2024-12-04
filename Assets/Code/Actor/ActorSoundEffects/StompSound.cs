using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class StompSound : ActorSound
    {
        [SerializeField] private EventReference stompLoopSound;
        [SerializeField] private EventReference stompLandSound;

        private EventInstance _stompLoopInstance;

        public override void Initialize()
        {
            base.Initialize();
            
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

            if (obj is FStateGround or FStateIdle or FStateSpecialJump && actor.stateMachine.PreviousState is FStateStomp)
            {
                RuntimeManager.PlayOneShot(stompLandSound);
            }
        }
    }
}