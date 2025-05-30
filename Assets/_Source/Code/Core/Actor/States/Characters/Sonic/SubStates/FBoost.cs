using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using SurgeEngine.Code.Gameplay.Inputs;
using SurgeEngine.Code.Infrastructure.Config;
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

        public bool CanAirBoost ;
        public bool RestoringTopSpeed;

        private float _boostEnergy;
        private IBoostHandler _boostHandler;
        private Coroutine _cancelBoostCoroutine;

        private readonly BoostConfig _config;

        private float _boostCancelTimer;
        private const float EnemyEnergyAddition = 10;

        public FBoost(ActorBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
            
            CanAirBoost = true;
            BoostEnergy = MaxBoostEnergy;
            
            Actor.input.BoostAction += BoostAction;
            Actor.stateMachine.OnStateAssign += OnStateAssign;

            ObjectEvents.OnObjectCollected += OnRingCollected;
            ObjectEvents.OnEnemyDied += OnEnemyDied;
        }
        
        ~FBoost()
        {
            Actor.input.BoostAction -= BoostAction;
            Actor.stateMachine.OnStateAssign -= OnStateAssign;

            ObjectEvents.OnObjectCollected -= OnRingCollected;
            ObjectEvents.OnEnemyDied -= OnEnemyDied;
        }

        private void OnRingCollected(ContactBase obj)
        {
            if (obj is Ring)
            {
                BoostEnergy += _config.RingEnergyAddition;
            }
        }

        private void OnEnemyDied(EnemyBase obj)
        {
            BoostEnergy += EnemyEnergyAddition;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is IBoostHandler casted)
            {
                _boostHandler = casted;
            }
            else
            {
                _boostHandler = null;
            }
            
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
                    Actor.StopCoroutine(_cancelBoostCoroutine);
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _boostHandler?.BoostHandle();

            FState state = Actor.stateMachine.CurrentState;
            FState prev = Actor.stateMachine.PreviousState;

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
                
                if (BoostEnergy > 0)
                {
                    BoostEnergy -= _config.EnergyDrain * Time.deltaTime;
                }
                else
                {
                    Active = false;
                }
                
                Actor.kinematics.TurnRate *= _config.TurnSpeedMultiplier;
            }
            
            BoostEnergy = Mathf.Clamp(BoostEnergy, 0, 100);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Active)
                HurtBox.CreateAttached(Actor, Actor.transform, new Vector3(0f, 0f, -0.1f), new Vector3(0.75f, 1f, 1.15f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
        }

        public bool CanBoost() => BoostEnergy > 0 && _boostHandler != null;

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (Actor.stateMachine.CurrentState is FStateAir && !CanAirBoost) return;
            if (Actor.stateMachine.CurrentState is FStateStomp) return;
            if (Actor.stateMachine.CurrentState is FStateSlide) return;
            
            if (CanBoost())
            {
                Active = obj.started && !Actor.flags.HasFlag(FlagType.OutOfControl);
            }
            
            if (Active)
            {
                Rigidbody body = Actor.kinematics.Rigidbody;
                float startSpeed = _config.StartSpeed;

                if (Actor.kinematics.HorizontalSpeed < startSpeed)
                {
                    if (Actor.stateMachine.CurrentState is FStateIdle)
                    {
                        Actor.stateMachine.SetState<FStateGround>();
                    }
                    
                    body.linearVelocity = body.transform.forward * startSpeed;
                    RestoringTopSpeed = true;
                }
                
                BoostEnergy -= _config.StartDrain;
                new Rumble().Vibrate(0.7f, 0.8f, 0.5f);
            }
        }
        
        public BoostConfig GetConfig() => _config;

        public void BaseGroundBoost()
        {
            float dt = Time.deltaTime;
            BaseActorConfig config = Actor.config;
            Rigidbody body = Actor.kinematics.Rigidbody;
            float speed = Actor.kinematics.HorizontalSpeed;
            
            if (Active)
            {
                if (Actor.input.moveVector == Vector3.zero) Actor.kinematics.SetInputDir(Actor.transform.forward);
                float maxSpeed = config.maxSpeed * _config.MaxSpeedMultiplier;
                if (speed < maxSpeed) body.AddForce(body.linearVelocity.normalized * (_config.Acceleration * dt), ForceMode.Impulse);
            }
        }
    }
}