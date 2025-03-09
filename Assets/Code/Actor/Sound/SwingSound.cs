using FMODUnity;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Actor.Sound
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