using System;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Infrastructure.Config;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class CharacterLife : CharacterComponent, IDamageable, IPointMarkerLoader
    {
        [SerializeField] private float invincibleTime = 4f;
        [SerializeField] private float directionalForce = 6f;
        public float DirectionalForce => directionalForce;

        public bool IsDead { get; set; }
        public event Action<CharacterBase> OnDied;
        public event Action OnRingLoss;
        
        public void TakeDamage(Component sender)
        {
            CharacterDamage damageable = character.StateMachine.CurrentState switch
            {
                FStateGrind or FStateGrindSquat => new GrindDamage(character),
                _ => new GeneralDamage(character)
            };
            
            if (!character.Flags.HasFlag(FlagType.Invincible))
            {
                IsDead = false;
                
                // Imagine it's over
                if (Stage.Instance.Data.RingCount <= 0)
                {
                    if (damageable is GeneralDamage) character.StateMachine.GetState<FStateDamage>()?.SetState(DamageState.Dead);

                    OnDiedInvoke(character, true);
                }
                else
                {
                    // Lose rings
                    const int min = 15;

                    var data = Stage.Instance.Data;
                    var ringCount = data.RingCount;

                    int value = Mathf.CeilToInt(Mathf.Max(min, ringCount * Random.Range(0.5f, 0.8f)));
                    data.RingCount -= Mathf.Clamp(value, 0, ringCount);
                    
                    OnRingLoss?.Invoke();
                }
                
                damageable.TakeDamage();
                character.Flags.AddFlag(new Flag(FlagType.Invincible, true, invincibleTime));
            }
        }
        
        public virtual void OnDiedInvoke(CharacterBase obj, bool isMarkedForDeath)
        {
            IsDead = isMarkedForDeath;
            OnDied?.Invoke(obj);
        }

        public void Load()
        {
            IsDead = false;
        }
    }
    
    abstract class CharacterDamage
    {
        protected readonly CharacterBase character;

        protected CharacterDamage(CharacterBase character) => this.character = character;
        
        public virtual void TakeDamage()
        {
            
        }
    }

    class GeneralDamage : CharacterDamage
    {
        public GeneralDamage(CharacterBase character) : base(character) { }

        public override void TakeDamage()
        {
            character.StateMachine.SetState<FStateDamage>()?.SetState(character.Life.IsDead ? DamageState.Dead : DamageState.Alive);
        }
    }

    class GrindDamage : CharacterDamage
    {
        public GrindDamage(CharacterBase character) : base(character) { }
    }
}