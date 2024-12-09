using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class ActorSound : MonoBehaviour
    {
        protected Actor actor => ActorContext.Context;
        protected VoiceHandler voice;
        
        public virtual void Initialize()
        {
            
        }

        private void Awake()
        {
            voice = GetComponent<VoiceHandler>();
        }

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += SoundState;
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= SoundState;
        }

        protected virtual void SoundState(FState obj) { }
    }
}