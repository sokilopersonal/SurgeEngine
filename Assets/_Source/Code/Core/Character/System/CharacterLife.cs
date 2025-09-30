using System;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine._Source.Code.Core.Character.System
{
    public class CharacterLife : CharacterComponent, IDamageable, IPointMarkerLoader
    {
        [SerializeField] private float invincibleTime = 4f;
        [SerializeField] private float directionalForce = 6f;
        public float DirectionalForce => directionalForce;

        public bool IsDead { get; set; }
        public bool WillDie => Stage.Instance != null && Stage.Instance.Data.RingCount <= 0;
        public Action<CharacterBase> OnDied;
        public event Action OnRingLoss;

        private void OnEnable()
        {
            OnDied += MarkAsDead;
        }

        private void OnDisable()
        {
            OnDied -= MarkAsDead;
        }

        public void TakeDamage(Component sender) => TakeDamage(true);

        public void TakeDamage(bool changeState)
        {
            CharacterDamage damageable = character.StateMachine.CurrentState switch
            {
                FStateGrind or FStateGrindSquat => new GrindDamage(character),
                _ => new GeneralDamage(character)
            };

            if (!character.Flags.HasFlag(FlagType.Invincible) && !IsDead)
            {
                IsDead = false;

                var stage = Stage.Instance;
                if (WillDie)
                {
                    if (damageable is GeneralDamage && changeState)
                        character.StateMachine.GetState<FStateDamage>()?.SetState(DamageState.Dead);

                    OnDied?.Invoke(character);
                }
                else
                {
                    const int min = 15;
                    var data = stage.Data;
                    var ringCount = data.RingCount;

                    int value = Mathf.CeilToInt(Mathf.Max(min, ringCount * Random.Range(0.5f, 0.8f)));
                    data.RingCount -= Mathf.Clamp(value, 0, ringCount);
                    OnRingLoss?.Invoke();
                }

                if (changeState) damageable.TakeDamage();
                character.Flags.AddFlag(new Flag(FlagType.Invincible, true, invincibleTime + 1.5f));
            }
        }

        private void MarkAsDead(CharacterBase obj)
        {
            IsDead = true;
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