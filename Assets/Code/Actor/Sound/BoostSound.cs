using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class BoostSound : ActorSound
    {
        [SerializeField] private EventReference boostSound;
        [SerializeField] private EventReference boostLoopSound;
        [SerializeField] private EventReference boostVoiceSound;
        [SerializeField] private BoostAudioDistortion boostAudioDistortion;
        
        private EventInstance _boostSoundInstance;
        private EventInstance _boostLoopInstance;
        private EventInstance _boostVoiceInstance;
        
        private float _lastBoostVoiceTime;

        public override void Initialize()
        {
            base.Initialize();
            
            _boostSoundInstance = RuntimeManager.CreateInstance(boostSound);
            _boostLoopInstance = RuntimeManager.CreateInstance(boostLoopSound);
            _boostVoiceInstance = RuntimeManager.CreateInstance(boostVoiceSound);
            //actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
        }

        private void OnBoostActivate(FSubState arg1, bool arg2)
        {
            if (arg2)
            {
                _boostSoundInstance.start();
                _boostLoopInstance.start();

                voice.Play(_boostVoiceInstance);
                
                boostAudioDistortion.Toggle();
            }
            else
            {
                _boostSoundInstance.stop(STOP_MODE.ALLOWFADEOUT);
                _boostLoopInstance.stop(STOP_MODE.ALLOWFADEOUT);
                
                boostAudioDistortion.Toggle();
            }
        }
    }
}