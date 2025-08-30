using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    [RequireComponent(typeof(VoiceHandler), typeof(CharacterSounds))]
    public abstract class CharacterSound : MonoBehaviour
    {
        protected CharacterBase Character;
        protected VoiceHandler Voice;
        
        public virtual void Initialize(CharacterBase character)
        {
            Character = character;
        }

        private void Awake()
        {
            Voice = GetComponent<VoiceHandler>();
        }

        protected virtual void OnEnable()
        {
            var stateMachine = Character.StateMachine;
            stateMachine.OnStateAssign += SoundState;
            stateMachine.OnStateEarlyAssign += EarlySoundState;
        }

        protected virtual void OnDisable()
        {
            var stateMachine = Character.StateMachine;
            stateMachine.OnStateAssign -= SoundState;
            stateMachine.OnStateEarlyAssign -= EarlySoundState;
        }

        protected virtual void SoundState(FState obj) { }

        protected virtual void EarlySoundState(FState obj) { }
    }
}