using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class SweepKickSound : ActorSound
    {
        [SerializeField] private EventReference sweepKickSound;
        [SerializeField] private EventReference sweepKickVoice;

        private EventInstance _sweep;

        public override void Initialize(ActorBase actor)
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