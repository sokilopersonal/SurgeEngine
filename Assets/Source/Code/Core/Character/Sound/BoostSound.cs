using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class BoostSound : CharacterSound
    {
        [SerializeField] private EventReference boostSound;
        [SerializeField] private EventReference boostLoopSound;
        [SerializeField] private EventReference boostVoiceSound;
        [SerializeField] private BoostAudioDistortion boostAudioDistortion;
        
        private EventInstance _boostSoundInstance;
        private EventInstance _boostLoopInstance;
        private EventInstance _boostVoiceInstance;
        
        private float _lastBoostVoiceTime;

        public override void Initialize(CharacterBase character)
        {
            base.Initialize(character);
            
            _boostSoundInstance = RuntimeManager.CreateInstance(boostSound);
            _boostLoopInstance = RuntimeManager.CreateInstance(boostLoopSound);
            _boostVoiceInstance = RuntimeManager.CreateInstance(boostVoiceSound);
            
            // Character.StateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
            
            if (character.StateMachine.GetState(out FBoost boost))
                boost.OnActiveChanged += OnBoostActivate;
        }

        private void OnBoostActivate(FSubState arg1, bool arg2)
        {
            if (arg2)
            {
                _boostSoundInstance.start();
                _boostLoopInstance.start();

                Voice.Play(_boostVoiceInstance);
                
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