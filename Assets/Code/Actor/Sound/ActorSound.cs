using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class ActorSound : MonoBehaviour
    {
        protected Actor Actor;
        protected VoiceHandler Voice;
        
        public virtual void Initialize(Actor actor)
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