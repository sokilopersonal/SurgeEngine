using FMODUnity;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class DamageSound : ActorSound
    {
        [SerializeField] private EventReference damageVoice;
        [SerializeField] private EventReference deathSound;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Actor.OnDied += OnDied;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Actor.OnDied -= OnDied;
        }

        private void OnDied(ActorBase actor)
        {
            Voice.Play(actor.IsDead ? deathSound : damageVoice, true);
        }
    }
}