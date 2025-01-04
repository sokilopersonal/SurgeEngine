using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class SlideSound : ActorSound
    {
        [SerializeField] private EventReference slideLoopSound;
        
        private EventInstance slideLoop;

        public override void Initialize()
        {
            base.Initialize();
            
            slideLoop = RuntimeManager.CreateInstance(slideLoopSound);
            slideLoop.set3DAttributes(transform.To3DAttributes());
        }

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSlide)
            {
                slideLoop.set3DAttributes(transform.To3DAttributes());
                slideLoop.start();
            }
            else
            {
                slideLoop.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}