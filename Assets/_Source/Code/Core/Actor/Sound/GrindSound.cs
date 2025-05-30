using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class GrindSound : ActorSound
    {
        [SerializeField] private EventReference grindStart;
        [SerializeField] private EventReference grindLoop;
        
        private EventInstance _grindLoopInstance;

        public override void Initialize(ActorBase actor)
        {
            base.Initialize(actor);
            
            _grindLoopInstance = RuntimeManager.CreateInstance(grindLoop);
        }

        private void Update()
        {
            _grindLoopInstance.set3DAttributes(gameObject.To3DAttributes());
        }

        protected override void SoundState(FState obj)
        {
            FState prev = Actor.StateMachine.PreviousState;
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