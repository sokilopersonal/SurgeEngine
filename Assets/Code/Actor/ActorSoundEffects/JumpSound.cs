using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class JumpSound : ActorSound
    {
        [SerializeField] private EventReference _spinSound;
        [SerializeField] private EventReference _voiceSound;

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= OnStateAssign;
        }
        
        private void OnStateAssign(FState obj)
        {
            if (obj is FStateJump)
            {
                RuntimeManager.PlayOneShot(_voiceSound);
                RuntimeManager.PlayOneShot(_spinSound);
            }
        }
    }
}