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
    public class SlideSound : ActorSound
    {
        [SerializeField] private EventReference slideLoopSound;
        [SerializeField] private EventReference slideVoice;

        private EventInstance _slideLoop;

        public override void Initialize(ActorBase actor)
        {
            base.Initialize(actor);
            
            _slideLoop = RuntimeManager.CreateInstance(slideLoopSound);
            _slideLoop.set3DAttributes(transform.To3DAttributes());
        }

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSlide)
            {
                _slideLoop.set3DAttributes(transform.To3DAttributes());
                _slideLoop.start();
                if (Actor.stateMachine.PreviousState is FStateGround)
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