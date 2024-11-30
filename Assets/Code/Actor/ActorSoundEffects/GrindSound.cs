using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class GrindSound : ActorSound
    {
        [SerializeField] private EventReference grindStart;
        [SerializeField] private EventReference grindLoop;
        
        private EventInstance _grindLoopInstance;

        public override void Initialize()
        {
            base.Initialize();
            
            _grindLoopInstance = RuntimeManager.CreateInstance(grindLoop);
        }

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= OnStateAssign;
        }

        private void Update()
        {
            _grindLoopInstance.set3DAttributes(gameObject.To3DAttributes());
        }

        private void OnStateAssign(FState obj)
        {
            var prev = actor.stateMachine.PreviousState;
            if (obj is FStateGrind and not FStateGrindSquat && prev is not FStateGrindSquat)
            {
                RuntimeManager.PlayOneShotAttached(grindStart, gameObject);
                _grindLoopInstance.start();
            }
            
            if (obj is not FStateGrind)
            {
                _grindLoopInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}