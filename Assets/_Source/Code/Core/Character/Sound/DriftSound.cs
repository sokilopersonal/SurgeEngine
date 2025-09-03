using FMOD.Studio;
using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine._Source.Code.Core.Character.Sound
{
    public class DriftSound : CharacterSound
    {
        [SerializeField] private EventReference driftLoop;
        [SerializeField] private EventReference driftVoice;

        private EventInstance _driftLoopInstance;
        private bool _isWater;

        public override void Initialize(CharacterBase character)
        {
            base.Initialize(character);
            
            _driftLoopInstance = RuntimeManager.CreateInstance(driftLoop);
            character.Kinematics.GroundTag.Changed += (_, newTag) => _isWater = newTag == GroundTag.Water;  
        }

        private void Update()
        {
            _driftLoopInstance.setParameterByName("OnWater", _isWater ? 1 : 0);
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