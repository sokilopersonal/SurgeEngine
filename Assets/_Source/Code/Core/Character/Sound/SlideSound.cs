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
    public class SlideSound : CharacterSound
    {
        [SerializeField] private EventReference slideLoopSound;
        [SerializeField] private EventReference slideVoice;

        private EventInstance _slideLoop;

        public override void Initialize(CharacterBase character)
        {
            base.Initialize(character);
            
            _slideLoop = RuntimeManager.CreateInstance(slideLoopSound);
            _slideLoop.set3DAttributes(transform.To3DAttributes());
        }

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSlide)
            {
                _slideLoop.set3DAttributes(transform.To3DAttributes());
                _slideLoop.start();
                if (Character.StateMachine.PreviousState is FStateGround)
                {
                    Voice.Play(slideVoice);
                }
            }
            else
            {
                _slideLoop.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}