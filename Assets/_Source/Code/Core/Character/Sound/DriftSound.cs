using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class DriftSound : CharacterSound
    {
        [SerializeField] private EventReference driftLoop;
        [SerializeField] private EventReference driftVoice;

        private EventInstance _driftLoopInstance;

        public override void Initialize(CharacterBase character)
        {
            base.Initialize(character);
            
            _driftLoopInstance = RuntimeManager.CreateInstance(driftLoop);
        }

        private void Update()
        {
            _driftLoopInstance.setParameterByName("OnWater", Character.StateMachine.GetState<FStateGround>().GetSurfaceTag() == GroundTag.Water ? 1 : 0);
        }

        protected override void SoundState(FState obj)
        {
            FState prev = Character.StateMachine.PreviousState;
            if (obj is FStateDrift)
            {
                _driftLoopInstance.start();
            }
            else
            {
                _driftLoopInstance.stop(STOP_MODE.ALLOWFADEOUT);
            }
            
            if (prev is FStateDrift)
            {
                Voice.Play(driftVoice);
            }
        }
    }
}