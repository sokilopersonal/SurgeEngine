using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.Parameters;
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

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= OnStateAssign;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is FStateSliding)
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