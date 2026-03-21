using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class SwingSound : CharacterSound
    {
        [SerializeField] private EventReference catchSound;
        [SerializeField] private EventReference swingSound;
        [SerializeField] private EventReference jumpSound;

        private EventInstance _swingLoopInstance;

        public override void Initialize(CharacterBase character)
        {
            base.Initialize(character);

            _swingLoopInstance = RuntimeManager.CreateInstance(swingSound);
        }

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSwing)
            {
                _swingLoopInstance.start();
                RuntimeManager.PlayOneShot(catchSound);
            }
            else if (obj is FStateSwingJump)
            {
                _swingLoopInstance.stop(STOP_MODE.ALLOWFADEOUT);
                RuntimeManager.PlayOneShot(jumpSound);
            }
        }
    }
}