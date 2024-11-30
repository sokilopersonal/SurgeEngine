using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.Custom;
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
        [SerializeField] private float boostVoiceDelay = 1.5f;
        
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
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;

            _lastBoostVoiceTime = Time.time - boostVoiceDelay;
        }

        private void OnBoostActivate(FSubState arg1, bool arg2)
        {
            if (arg2)
            {
                _boostSoundInstance.start();
                _boostLoopInstance.start();

                if (Common.InDelayTime(_lastBoostVoiceTime, boostVoiceDelay))
                {
                    _boostVoiceInstance.stop(STOP_MODE.IMMEDIATE);
                    _boostVoiceInstance.start();
                    _lastBoostVoiceTime = Time.time;
                }
                
                boostAudioDistortion.Toggle();
            }
            else
            {
                _boostSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _boostLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                
                boostAudioDistortion.Toggle();
            }
        }
    }
}