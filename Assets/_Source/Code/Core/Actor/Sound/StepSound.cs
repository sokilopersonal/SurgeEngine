using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class StepSound : ActorSound
    {
        [SerializeField] private EventReference stepSound;
        [SerializeField] private EventReference crawlSound;
        [SerializeField] private EventReference landSound;
        
        private EventInstance _stepSoundInstance;
        private EventInstance _crawlSoundInstance;
        private EventInstance _landSoundInstance;
        
        private const float LandSoundActivationThreshold = -5;

        public override void Initialize(ActorBase actor)
        {
            base.Initialize(actor);
            
            _stepSoundInstance = RuntimeManager.CreateInstance(stepSound);
            _stepSoundInstance.set3DAttributes(transform.To3DAttributes());

            _crawlSoundInstance = RuntimeManager.CreateInstance(crawlSound);
            _crawlSoundInstance.set3DAttributes(transform.To3DAttributes());
            _crawlSoundInstance.setVolume(0.1f);

            _landSoundInstance = RuntimeManager.CreateInstance(landSound);
            _landSoundInstance.set3DAttributes(transform.To3DAttributes());
        }

        public void PlaySound()
        {
            if (_stepSoundInstance.isValid() && Actor.stateMachine.CurrentState is FStateGround)
            {
                RuntimeManager.AttachInstanceToGameObject(_stepSoundInstance, transform);
                _stepSoundInstance.setParameterByNameWithLabel("GroundTag", Actor.stateMachine.GetState<FStateGround>().GetSurfaceTag().ToString());
                _stepSoundInstance.start();
            }
            if (_crawlSoundInstance.isValid() && Actor.stateMachine.CurrentState is FStateCrawl)
            {
                RuntimeManager.AttachInstanceToGameObject(_crawlSoundInstance, transform);
                _crawlSoundInstance.start();
            }
        }

        protected override void SoundState(FState obj)
        {
            FStateMachine machine = Actor.stateMachine;
            RuntimeManager.AttachInstanceToGameObject(_landSoundInstance, transform);
            if (machine.IsExact<FStateGround>() || machine.IsExact<FStateIdle>())
            {
                FState prev = machine.PreviousState;

                if (prev is FStateAir && Actor.kinematics.Velocity.y < LandSoundActivationThreshold)
                {
                    _landSoundInstance.start();
                }
            }
        }
    }
}