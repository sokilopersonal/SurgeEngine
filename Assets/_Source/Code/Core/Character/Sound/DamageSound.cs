using FMODUnity;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class DamageSound : CharacterSound
    {
        [SerializeField] private EventReference damageVoice;
        [SerializeField] private EventReference deathSound;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Character.Life.OnDied += OnDied;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Character.Life.OnDied -= OnDied;
        }

        private void OnDied(CharacterBase character)
        {
            Voice.Play(character.Life.IsDead ? deathSound : damageVoice, true);
        }
    }
}