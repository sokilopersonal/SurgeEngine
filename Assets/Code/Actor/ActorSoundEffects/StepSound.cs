using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class StepSound : ActorSound
    {
        [SerializeField] private EventReference stepSound;
        [SerializeField] private EventReference landSound;
        
        private EventInstance _stepSoundInstance;
        private EventInstance _landSoundInstance;

        public override void Initialize()
        {
            base.Initialize();
            
            _stepSoundInstance = RuntimeManager.CreateInstance(stepSound);
            _stepSoundInstance.set3DAttributes(transform.To3DAttributes());
            
            _landSoundInstance = RuntimeManager.CreateInstance(landSound);
            _landSoundInstance.set3DAttributes(transform.To3DAttributes());
        }

        public void PlaySound()
        {
            if (_stepSoundInstance.isValid() && actor.stateMachine.CurrentState is FStateGround)
            {
                RuntimeManager.AttachInstanceToGameObject(_stepSoundInstance, transform);
                _stepSoundInstance.setParameterByNameWithLabel("GroundTag", actor.stateMachine.GetState<FStateGround>().GetSurfaceTag());
                _stepSoundInstance.start();
            }
        }

        protected override void SoundState(FState obj)
        {
            var machine = actor.stateMachine;
            RuntimeManager.AttachInstanceToGameObject(_landSoundInstance, transform);
            if (machine.IsExact<FStateGround>() || machine.IsExact<FStateIdle>())
            {
                var prev = machine.PreviousState;

                if (prev is FStateAir)
                {
                    _landSoundInstance.start();
                }
            }
        }
    }
}