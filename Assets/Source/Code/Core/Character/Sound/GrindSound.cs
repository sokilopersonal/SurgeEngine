using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class GrindSound : CharacterSound
    {
        [SerializeField] private EventReference grindStart;
        [SerializeField] private EventReference grindLoop;
        
        private EventInstance _grindLoopInstance;

        public override void Initialize(CharacterBase character)
        {
            base.Initialize(character);
            
            _grindLoopInstance = RuntimeManager.CreateInstance(grindLoop);
        }

        private void Update()
        {
            _grindLoopInstance.set3DAttributes(gameObject.To3DAttributes());
        }

        protected override void SoundState(FState obj)
        {
            FState prev = Character.StateMachine.PreviousState;
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