using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Gameplay.Inputs;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates
{
    public class FBoost : FCharacterSubState
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
        private float _boostKeepTimer;
        private float _boostNoEnergyCancelTimer;

        public FBoost(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
            
            CanAirBoost = true;
            BoostEnergy = MaxBoostEnergy * _config.StartBoostCapacity;
            
            owner.Input.XAction += BoostAction;
            Character.StateMachine.OnStateAssign += OnStateAssign;

            ObjectEvents.OnObjectTriggered += OnRingCollected;
            ObjectEvents.OnEnemyDied += OnEnemyDied;
        }
        
        ~FBoost()
        {
            Character.Input.XAction -= BoostAction;
            Character.StateMachine.OnStateAssign -= OnStateAssign;

            ObjectEvents.OnObjectTriggered -= OnRingCollected;
            ObjectEvents.OnEnemyDied -= OnEnemyDied;
        }

        private void OnRingCollected(StageObject obj)
        {
            if (obj is Ring ring)
            {
                BoostEnergy += _config.RingEnergyAddition * (!ring.IsSuperRing ? 1 : 5);
            }
        }

        private void OnEnemyDied(EnemyBase obj)
        {
            BoostEnergy += 10;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _boostHandler?.BoostHandle(Character, _config);

            FState state = Character.StateMachine.CurrentState;
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

                if (Character.Kinematics.Speed < _config.MaxBoostSpeed / 8)
                {
                    _boostKeepTimer += dt / _config.KeepTime;
                    if (_boostKeepTimer >= 1f)
                    {
                        Active = false;
                    }
                }
                else
                {
                    _boostKeepTimer = 0;
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
                
                Character.Kinematics.TurnRate *= _config.TurnSpeedMultiplier;
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
                    Character.StopCoroutine(_cancelBoostCoroutine);
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
            HurtBox.CreateAttached(Character, Character.transform, new Vector3(0f, 0f, -0.1f), new Vector3(0.5f, 1f, 1.15f), 
                HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
        }

        private void FindRings()
        {
            var mask = _config.MagnetRingMask;
            var radius = _config.MagnetRadius;
            
            var col = Physics.OverlapSphere(Character.transform.position, radius, mask);
            if (col.Length > 0)
            {
                for (int i = 0; i < col.Length; i++)
                {
                    col[i].GetComponent<Ring>().StartMagnet(Character);
                }
            }
        }

        public bool CanBoost() => BoostEnergy > 0;

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (Character.StateMachine.CurrentState is FStateAir && !CanAirBoost) return;

            if (_boostHandler == null)
            {
                if (Active)
                    Active = false;
                
                return;
            }
            
            if (CanBoost())
            {
                Active = obj.started && !Character.Flags.HasFlag(FlagType.OutOfControl);
            }
            
            if (Active)
            {
                Rigidbody body = Character.Kinematics.Rigidbody;
                float startSpeed = _config.StartSpeed;
                
                if (Character.Kinematics.Speed < startSpeed)
                {
                    body.linearVelocity = body.transform.forward * startSpeed;
                }
                
                BoostEnergy -= _config.StartDrain;
                new Rumble().Vibrate(0.7f, 0.8f, 0.5f);
            }
        }
    }
}