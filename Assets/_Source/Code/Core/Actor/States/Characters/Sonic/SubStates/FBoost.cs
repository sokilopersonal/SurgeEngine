using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using SurgeEngine.Code.Gameplay.Inputs;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates
{
    public class FBoost : FActorSubState
    {
        public float BoostEnergy
        {
            get => _boostEnergy; 
            set => _boostEnergy = Mathf.Clamp(value, 0, MaxBoostEnergy);
        }

        public float MaxBoostEnergy => _config.BoostCapacity;

        public bool CanAirBoost;

        private float _boostEnergy;
        private IBoostHandler _boostHandler;
        private Coroutine _cancelBoostCoroutine;

        private readonly BoostConfig _config;

        private float _boostCancelTimer;
        private float _boostLowSpeedCancelTimer;
        private float _boostNoEnergyCancelTimer;
        private const float EnemyEnergyAddition = 10;

        public FBoost(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
            
            CanAirBoost = true;
            BoostEnergy = MaxBoostEnergy * _config.StartBoostCapacity;
            
            owner.Input.XAction += BoostAction;
            character.StateMachine.OnStateAssign += OnStateAssign;

            ObjectEvents.OnObjectTriggered += OnRingCollected;
            ObjectEvents.OnEnemyDied += OnEnemyDied;
        }
        
        ~FBoost()
        {
            character.Input.XAction -= BoostAction;
            character.StateMachine.OnStateAssign -= OnStateAssign;

            ObjectEvents.OnObjectTriggered -= OnRingCollected;
            ObjectEvents.OnEnemyDied -= OnEnemyDied;
        }

        private void OnRingCollected(ContactBase obj)
        {
            if (obj is Ring ring)
            {
                BoostEnergy += _config.RingEnergyAddition * (!ring.IsSuperRing() ? 1 : 10);
            }
        }

        private void OnEnemyDied(EnemyBase obj)
        {
            BoostEnergy += EnemyEnergyAddition;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _boostHandler?.BoostHandle(character, _config);

            FState state = character.StateMachine.CurrentState;
            if (state is FStateDrift)
            {
                BoostEnergy += _config.DriftEnergyAddition * dt;
            }

            if (Active)
            {
                if (state is FStateAir or FStateSpecialJump)
                {
                    _boostCancelTimer += dt;
                
                    if (_boostCancelTimer >= _config.InAirTime)
                    {
                        Active = false;
                    }
                }
                else
                {
                    _boostCancelTimer = 0;
                }

                if (state is FStateGround)
                {
                    if (character.Kinematics.Speed < _config.MaxBoostSpeed / 8)
                    {
                        _boostLowSpeedCancelTimer += dt / 0.35f;
                        if (_boostLowSpeedCancelTimer >= 1f)
                        {
                            Active = false;
                        }
                    }
                    else
                    {
                        _boostLowSpeedCancelTimer = 0;
                    }
                }
                else
                {
                    _boostLowSpeedCancelTimer = 0;
                }
                
                if (BoostEnergy > 0)
                {
                    BoostEnergy -= _config.EnergyDrain * Time.deltaTime;
                }
                else
                {
                    if (_boostNoEnergyCancelTimer >= 1f)
                    {
                        Active = false;
                        _boostNoEnergyCancelTimer = 0;
                    }
                }

                if (BoostEnergy <= 0)
                {
                    _boostNoEnergyCancelTimer += dt / 0.1f;
                }
                
                character.Kinematics.TurnRate *= _config.TurnSpeedMultiplier;
            }
            else
            {
                _boostNoEnergyCancelTimer = 0;
            }
            
            BoostEnergy = Mathf.Clamp(BoostEnergy, 0, MaxBoostEnergy);
        }

        private void OnStateAssign(FState obj)
        {
            _boostHandler = obj switch
            {
                FStateIdle or FStateGround or FStateDrift or FStateGrind => new BoostGroundHandle(),
                FStateAir => new BoostAirHandle(),
                _ => null
            };

            if (obj is FStateGround)
            {
                CanAirBoost = true;
            }

            if (obj is FStateAir)
            {
                if (CanAirBoost)
                {
                    _boostCancelTimer = 0;
                }
            }
            
            if (obj is FStateGrind)
            {
                if (_cancelBoostCoroutine != null)
                    character.StopCoroutine(_cancelBoostCoroutine);
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Active)
            {
                CreateDamage();
                FindRings();
            }
        }

        private void CreateDamage()
        {
            HurtBox.CreateAttached(character, character.transform, new Vector3(0f, 0f, -0.1f), new Vector3(0.75f, 1f, 1.15f), 
                HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
        }

        private void FindRings()
        {
            var mask = _config.MagnetRingMask;
            var radius = _config.MagnetRadius;
            
            var col = Physics.OverlapSphere(character.transform.position, radius, mask);
            if (col.Length > 0)
            {
                for (int i = 0; i < col.Length; i++)
                {
                    col[i].GetComponent<Ring>().StartMagnet(character);
                }
            }
        }

        public bool CanBoost() => BoostEnergy > 0;

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (character.StateMachine.CurrentState is FStateAir && !CanAirBoost) return;

            if (_boostHandler == null)
            {
                if (Active)
                    Active = false;
                
                return;
            }
            
            if (CanBoost())
            {
                Active = obj.started && !character.Flags.HasFlag(FlagType.OutOfControl);
            }
            
            if (Active)
            {
                Rigidbody body = character.Kinematics.Rigidbody;
                float startSpeed = _config.StartSpeed;
                
                if (character.Kinematics.Speed < startSpeed)
                {
                    body.linearVelocity = body.transform.forward * startSpeed;
                }
                
                BoostEnergy -= _config.StartDrain;
                new Rumble().Vibrate(0.7f, 0.8f, 0.5f);
            }
        }
    }
}