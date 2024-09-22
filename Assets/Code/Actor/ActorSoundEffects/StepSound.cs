using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class StepSound : ActorSound
    {
        [SerializeField] private EventReference stepSound;
        
        private EventInstance _stepSoundInstance;

        public override void Initialize()
        {
            base.Initialize();
            
            _stepSoundInstance = RuntimeManager.CreateInstance(stepSound);
            _stepSoundInstance.set3DAttributes(transform.To3DAttributes());
        }

        public void Play()
        {
            if (_stepSoundInstance.isValid())
            {
                RuntimeManager.AttachInstanceToGameObject(_stepSoundInstance, transform);
                _stepSoundInstance.setParameterByNameWithLabel("GroundTag", actor.stateMachine.GetState<FStateGround>().GetSurfaceTag());
                _stepSoundInstance.start();
            }
        }
    }
}