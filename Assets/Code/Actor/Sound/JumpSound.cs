using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.StateMachine;
using System.Collections;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
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
                voice.Play(_voiceSound);
                if (actor.stateMachine.IsPrevExact<FStateJump>())
                    RuntimeManager.PlayOneShot(_spinSound);
                else
                    StartCoroutine(SpinSound());
            }
        }

        private IEnumerator SpinSound()
        {
            RuntimeManager.PlayOneShot(_jumpSound);
            yield return new WaitForSeconds(0.117f);
            if (actor.input.JumpHeld)
            {
                RuntimeManager.PlayOneShot(_spinSound);
            }
        }
    }
}