using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class SweepKickSound : ActorSound
    {
        [SerializeField] private EventReference sweepKickSound;
        [SerializeField] private EventReference sweepKickVoice;

        private EventInstance sweep;
        private EventInstance voiceInstance;

        public override void Initialize()
        {
            base.Initialize();

            sweep = RuntimeManager.CreateInstance(sweepKickSound);
            sweep.set3DAttributes(transform.To3DAttributes());

            voiceInstance = RuntimeManager.CreateInstance(sweepKickVoice);
        }

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSweepKick)
            {
                sweep.set3DAttributes(transform.To3DAttributes());
                sweep.start();
                voiceInstance.start();
            }
        }
    }
}