using System.Collections;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class JumpSound : CharacterSound
    {
        [SerializeField] private EventReference _jumpSound;
        [SerializeField] private EventReference _spinSound;
        [SerializeField] private EventReference _voiceSound;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateJump)
            {
                Voice.Play(_voiceSound);
                if (Character.StateMachine.IsPrevExact<FStateJump>())
                    RuntimeManager.PlayOneShot(_spinSound);
                else
                    StartCoroutine(SpinSound());
            }
        }

        private IEnumerator SpinSound()
        {
            RuntimeManager.PlayOneShot(_jumpSound);
            yield return new WaitForSeconds(0.117f);
            if (Character.Input.AHeld)
            {
                RuntimeManager.PlayOneShot(_spinSound);
            }
        }
    }
}