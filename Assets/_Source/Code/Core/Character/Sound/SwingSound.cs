using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.Sound
{
    public class SwingSound : CharacterSound
    {
        [SerializeField] private EventReference catchSound;
        [SerializeField] private EventReference swingSound;
        [SerializeField] private EventReference jumpSound;

        public void Swing()
        {
            RuntimeManager.PlayOneShot(swingSound);
        }

        protected override void SoundState(FState obj)
        {
            if (obj is FStateSwing)
            {
                RuntimeManager.PlayOneShot(catchSound);
            }
            else if (obj is FStateSwingJump)
            {
                RuntimeManager.PlayOneShot(jumpSound);
            }
        }
    }
}