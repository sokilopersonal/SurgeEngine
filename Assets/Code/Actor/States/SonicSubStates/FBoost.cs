using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Enemy;
using SurgeEngine.Code.Misc;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.Actor.States.SonicSubStates
{
    public class FBoost : FActorSubState
    {
        public float BoostEnergy
        {
            get => _boostEnergy; 
            set => _boostEnergy = Mathf.Clamp(value, 0, 100);
        }
        
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
            CanAirBoost = true;
            BoostEnergy = 100;

            owner.TryGetConfig(out _config);
            
            actor.input.BoostAction += BoostAction;
            actor.stateMachine.OnStateAssign += OnStateAssign;

            ObjectEvents.OnObjectCollected += OnRingCollected;
            ObjectEvents.OnEnemyDied += OnEnemyDied;
        }
        
        ~FBoost()
        {
            actor.input.BoostAction -= BoostAction;
            actor.stateMachine.OnStateAssign -= OnStateAssign;

            ObjectEvents.OnObjectCollected -= OnRingCollected;
            ObjectEvents.OnEnemyDied -= OnEnemyDied;
        }

        private void OnRingCollected(ContactBase obj)
        {
            if (obj is Ring)
            {
                BoostEnergy += _config.ringEnergyAddition;
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
                    actor.StopCoroutine(_cancelBoostCoroutine);
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _boostHandler?.BoostHandle();

            FState state = actor.stateMachine.CurrentState;
            FState prev = actor.stateMachine.PreviousState;

            if (state is FStateDrift)
            {
                BoostEnergy += _config.driftEnergyAddition * dt;
            }

            if (Active)
            {
                if (state is FStateAir or FStateSpecialJump)
                {
                    _boostCancelTimer += dt;
                
                    if (_boostCancelTimer >= _config.inAirTime)
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
                    BoostEnergy -= _config.energyDrain * Time.deltaTime;
                }
                else
                {
                    Active = false;
                }
                
                actor.kinematics.TurnRate *= _config.turnSpeedMultiplier;
            }
            
            BoostEnergy = Mathf.Clamp(BoostEnergy, 0, 100);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Active)
                HurtBox.Create(actor, 
                    actor.transform.position + new Vector3(0f, 0.6f, -0.1f),
                    actor.transform.rotation, new Vector3(0.75f, 1f, 1.15f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
        }

        public bool CanBoost() => BoostEnergy > 0 && _boostHandler != null;

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (actor.stateMachine.CurrentState is FStateAir && !CanAirBoost) return;
            if (actor.stateMachine.CurrentState is FStateStomp) return;
            if (actor.stateMachine.CurrentState is FStateSlide) return;
            
            if (CanBoost())
            {
                Active = obj.started && !actor.flags.HasFlag(FlagType.OutOfControl);
            }
            
            if (Active)
            {
                Rigidbody body = actor.kinematics.Rigidbody;
                float startSpeed = _config.startSpeed;

                if (actor.kinematics.HorizontalSpeed < startSpeed)
                {
                    if (actor.stateMachine.CurrentState is FStateIdle)
                    {
                        actor.stateMachine.SetState<FStateGround>();
                    }
                    
                    body.linearVelocity = body.transform.forward * startSpeed;
                    RestoringTopSpeed = true;
                }
                
                BoostEnergy -= _config.startDrain;
                new Rumble().Vibrate(0.7f, 0.8f, 0.5f);
            }
        }
        
        public BoostConfig GetConfig() => _config;

        public void BaseGroundBoost()
        {
            float dt = Time.deltaTime;
            BaseActorConfig config = actor.config;
            Rigidbody body = actor.kinematics.Rigidbody;
            float speed = actor.kinematics.HorizontalSpeed;
            
            if (Active)
            {
                if (actor.input.moveVector == Vector3.zero) actor.kinematics.SetInputDir(actor.transform.forward);
                float maxSpeed = config.maxSpeed * _config.maxSpeedMultiplier;
                if (speed < maxSpeed) body.AddForce(body.linearVelocity.normalized * (_config.acceleration * dt), ForceMode.Impulse);
            }
            else if (RestoringTopSpeed)
            {
                float normalMaxSpeed = actor.config.topSpeed;
                if (speed > normalMaxSpeed)
                {
                    body.linearVelocity = Vector3.MoveTowards(
                        body.linearVelocity, 
                        body.transform.forward * normalMaxSpeed, 
                        dt * 16
                    );
                }
                else if (speed * 0.99f < normalMaxSpeed)
                {
                    RestoringTopSpeed = false;
                }
            }
        }
    }
}