using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Actor.Sound
{
    public class ActorSound : MonoBehaviour
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
            Actor.stateMachine.OnStateAssign += SoundState;
        }

        protected virtual void OnDisable()
        {
            Actor.stateMachine.OnStateAssign -= SoundState;
        }

        protected virtual void SoundState(FState obj) { }
    }
}