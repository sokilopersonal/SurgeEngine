using System.Collections;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class JumpSound : ActorSound
    {
        [SerializeField] private EventReference _jumpSound;
        [SerializeField] private EventReference _spinSound;
        [SerializeField] private EventReference _voiceSound;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateJump)
            {
                Voice.Play(_voiceSound);
                if (Actor.stateMachine.IsPrevExact<FStateJump>())
                    RuntimeManager.PlayOneShot(_spinSound);
                else
                    StartCoroutine(SpinSound());
            }
        }

        private IEnumerator SpinSound()
        {
            RuntimeManager.PlayOneShot(_jumpSound);
            yield return new WaitForSeconds(0.117f);
            if (Actor.input.JumpHeld)
            {
                RuntimeManager.PlayOneShot(_spinSound);
            }
        }
    }
}