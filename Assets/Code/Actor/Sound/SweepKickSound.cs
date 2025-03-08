using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class SweepKickSound : ActorSound
    {
        [SerializeField] private EventReference sweepKickSound;
        [SerializeField] private EventReference sweepKickVoice;

        private EventInstance _sweep;

        public override void Initialize(Actor actor)
        {
            base.Initialize(actor);

            _sweep = RuntimeManager.CreateInstance(sweepKickSound);
            _sweep.set3DAttributes(transform.To3DAttributes());
        }

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSweepKick)
            {
                _sweep.set3DAttributes(transform.To3DAttributes());
                _sweep.start();
                Voice.Play(sweepKickVoice);
            }
        }
    }
}