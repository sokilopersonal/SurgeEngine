using FMOD.Studio;
using FMODUnity;
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

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= OnStateAssign;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is FStateStomp)
            {
                _stompLoopInstance.start();
            }

            if (obj is FStateGround or FStateIdle && actor.stateMachine.PreviousState is FStateStomp)
            {
                RuntimeManager.PlayOneShot(stompLandSound);
                _stompLoopInstance.stop(STOP_MODE.IMMEDIATE);
            }
        }
    }
}