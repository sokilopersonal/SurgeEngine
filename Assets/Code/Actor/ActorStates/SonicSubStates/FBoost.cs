using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Misc;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.ActorStates.SonicSubStates
{
    public class FBoost : FActorSubState
    {
        public float BoostEnergy
        {
            get => _boostEnergy;
            private set => _boostEnergy = Mathf.Clamp(value, 0, 100);
        }
        
        public bool canAirBoost;
        
        public bool restoringTopSpeed;

        private float _boostEnergy;
        private IBoostHandler _boostHandler;
        private Coroutine _cancelBoostCoroutine;

        private readonly BoostConfig _config;

        private float _boostCancelTimer;

        public FBoost(Actor owner) : base(owner)
        {
            canAirBoost = true;
            BoostEnergy = 100;
            
            _config = (owner as Sonic).boostConfig;
            
            actor.input.BoostAction += BoostAction;
            actor.stateMachine.OnStateAssign += OnStateAssign;

            ObjectEvents.OnObjectCollected += _ => BoostEnergy += 
                _config.ringEnergyAddition;
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
                canAirBoost = true;
            }

            if (obj is FStateAir)
            {
                if (canAirBoost)
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

        public bool CanBoost() => BoostEnergy > 0 && !actor.stateMachine.IsExact<FStateSpecialJump>() && !actor.stateMachine.IsExact<FStateSwing>();

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (actor.stateMachine.CurrentState is FStateAir && !canAirBoost) return;
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
                    restoringTopSpeed = true;
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
            else if (restoringTopSpeed)
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
                    restoringTopSpeed = false;
                }
            }
        }
    }
}