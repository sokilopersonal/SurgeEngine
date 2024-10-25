using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class DriftSound : ActorSound
    {
        [SerializeField] private EventReference driftLoop;
        [SerializeField] private EventReference driftVoice;

        private EventInstance _driftLoopInstance;

        public override void Initialize()
        {
            base.Initialize();
            
            _driftLoopInstance = RuntimeManager.CreateInstance(driftLoop);
        }

        private void Update()
        {
            _driftLoopInstance.setParameterByName("OnWater", actor.stateMachine.GetState<FStateGround>().GetSurfaceTag() == "Water" ? 1 : 0);
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
            var prev = actor.stateMachine.PreviousState;
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
                RuntimeManager.PlayOneShot(driftVoice);
            }
        }
    }
}