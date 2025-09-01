using FMOD.Studio;
using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.Sound
{
    public class StepSound : CharacterSound
    {
        [SerializeField] private EventReference stepSound;
        [SerializeField] private EventReference crawlSound;
        [SerializeField] private EventReference landSound;
        
        private EventInstance _stepSoundInstance;
        private EventInstance _crawlSoundInstance;
        private EventInstance _landSoundInstance;
        
        private const float LandSoundActivationThreshold = -5;

        public override void Initialize(CharacterBase character)
        {
            base.Initialize(character);
            
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
            if (_stepSoundInstance.isValid() && Character.StateMachine.CurrentState is FStateGround)
            {
                RuntimeManager.AttachInstanceToGameObject(_stepSoundInstance, gameObject);
                _stepSoundInstance.setParameterByNameWithLabel("GroundTag", Character.StateMachine.GetState<FStateGround>().GetSurfaceTag().ToString());
                _stepSoundInstance.start();
            }
            if (_crawlSoundInstance.isValid() && Character.StateMachine.CurrentState is FStateCrawl)
            {
                RuntimeManager.AttachInstanceToGameObject(_crawlSoundInstance, gameObject);
                _crawlSoundInstance.start();
            }
        }

        protected override void SoundState(FState obj)
        {
            FStateMachine machine = Character.StateMachine;
            RuntimeManager.AttachInstanceToGameObject(_landSoundInstance, gameObject);
            if (machine.IsExact<FStateGround>() || machine.IsExact<FStateIdle>())
            {
                FState prev = machine.PreviousState;

                if (prev is FStateAir && Character.Kinematics.Velocity.y < LandSoundActivationThreshold)
                {
                    _landSoundInstance.start();
                }
            }
        }
    }
}