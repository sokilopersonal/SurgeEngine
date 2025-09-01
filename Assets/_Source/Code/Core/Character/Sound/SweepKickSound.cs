using FMOD.Studio;
using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.Sound
{
    public class SweepKickSound : CharacterSound
    {
        [SerializeField] private EventReference sweepKickSound;
        [SerializeField] private EventReference sweepKickVoice;

        private EventInstance _sweep;

        public override void Initialize(CharacterBase character)
        {
            base.Initialize(character);

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