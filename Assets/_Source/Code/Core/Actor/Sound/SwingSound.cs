using FMODUnity;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class SwingSound : ActorSound
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