using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    [RequireComponent(typeof(VoiceHandler), typeof(ActorSounds))]
    public abstract class ActorSound : MonoBehaviour
    {
        protected ActorBase Actor;
        protected VoiceHandler Voice;
        
        public virtual void Initialize(ActorBase actor)
        {
            Actor = actor;
        }

        private void Awake()
        {
            Voice = GetComponent<VoiceHandler>();
        }

        protected virtual void OnEnable()
        {
            var stateMachine = Actor.StateMachine;
            stateMachine.OnStateAssign += SoundState;
            stateMachine.OnStateEarlyAssign += EarlySoundState;
        }

        protected virtual void OnDisable()
        {
            var stateMachine = Actor.StateMachine;
            stateMachine.OnStateAssign -= SoundState;
            stateMachine.OnStateEarlyAssign -= EarlySoundState;
        }

        protected virtual void SoundState(FState obj) { }

        protected virtual void EarlySoundState(FState obj) { }
    }
}