using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.Actor.Sound
{
    public class DriftSound : ActorSound
    {
        [SerializeField] private EventReference driftLoop;
        [SerializeField] private EventReference driftVoice;

        private EventInstance _driftLoopInstance;

        public override void Initialize(ActorBase actor)
        {
            base.Initialize(actor);
            
            _driftLoopInstance = RuntimeManager.CreateInstance(driftLoop);
        }

        private void Update()
        {
            _driftLoopInstance.setParameterByName("OnWater", Actor.stateMachine.GetState<FStateGround>().GetSurfaceTag() == "Water" ? 1 : 0);
        }

        protected override void SoundState(FState obj)
        {
            FState prev = Actor.stateMachine.PreviousState;
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