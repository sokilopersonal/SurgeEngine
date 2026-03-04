using System.Collections;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;
using UnityEngine.Serialization;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class JumpSound : CharacterSound
    {
        [FormerlySerializedAs("_jumpSound")] [SerializeField] private EventReference jumpSound;
        [FormerlySerializedAs("_spinSound")] [SerializeField] private EventReference spinSound;
        [FormerlySerializedAs("_voiceSound")] [SerializeField] private EventReference voiceSound;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateJump)
            {
                Voice.Play(voiceSound);
                if (Character.StateMachine.IsPrevExact<FStateJump>())
                    RuntimeManager.PlayOneShot(spinSound);
                else
                    StartCoroutine(SpinSound());
            }
        }

        private IEnumerator SpinSound()
        {
            RuntimeManager.PlayOneShotAttached(jumpSound, Character.gameObject);
            yield return new WaitForSeconds(0.117f);
            if (Character.Input.AHeld)
            {
                RuntimeManager.PlayOneShot(spinSound);
            }
        }
    }
}