using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class JumpSound : ActorSound
    {
        [SerializeField] private EventReference _spinSound;
        [SerializeField] private EventReference _voiceSound;
        
        protected override void SoundState(FState obj)
        {
            if (obj is FStateJump)
            {
                voice.Play(_voiceSound);
                RuntimeManager.PlayOneShot(_spinSound);
            }
        }
    }
}