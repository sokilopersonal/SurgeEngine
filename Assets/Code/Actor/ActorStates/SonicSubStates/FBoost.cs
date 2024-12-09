using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
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
                    if (_cancelBoostCoroutine != null) 
                        actor.StopCoroutine(_cancelBoostCoroutine);

                    //_cancelBoostCoroutine = actor.StartCoroutine(CancelBoost(_boostEnergyGroup.GetParameter<float>(BoostEnergy_InAirTime)));
                }
            }
            
            if (obj is FStateAirBoost)
            {
                if (_cancelBoostCoroutine != null)
                    actor.StopCoroutine(_cancelBoostCoroutine);
                
                //_cancelBoostCoroutine = actor.StartCoroutine(CancelBoost(_boostEnergyGroup.GetParameter<float>(BoostEnergy_AirBoostTime)));
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

            if (actor.stateMachine.CurrentState is FStateGround)
            {
                if (!actor.flags.HasFlag(FlagType.OutOfControl))
                {
                    if (actor.input.BoostHeld)
                    {
                        if (_cancelBoostCoroutine != null)
                        {
                            actor.StopCoroutine(_cancelBoostCoroutine);
                        }

                        if (_cancelBoostCoroutine != null)
                        {
                            actor.StopCoroutine(_cancelBoostCoroutine);
                        }
                    }
                }
            }

            if (actor.stateMachine.CurrentState is FStateDrift)
            {
                BoostEnergy += _config.driftEnergyAddition * dt;
            }

            if (Active)
            {
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

            if (Common.CheckForGround(out _, CheckGroundType.Predict))
            {
                Active = false;
            }
            
            BoostEnergy = Mathf.Clamp(BoostEnergy, 0, 100);
        }

        public bool CanBoost() => BoostEnergy > 0 && !actor.stateMachine.IsExact<FStateSpecialJump>();

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (actor.stateMachine.CurrentState is FStateAir && !canAirBoost) return;
            if (actor.stateMachine.CurrentState is FStateStomp) return;
            if (actor.stateMachine.CurrentState is FStateSliding) return;
            
            if (CanBoost())
            {
                Active = obj.started;
                if (_cancelBoostCoroutine != null) 
                    actor.StopCoroutine(_cancelBoostCoroutine);
            }
            
            if (Active)
            {
                BoostEnergy -= _config.startDrain;
                new Rumble().Vibrate(0.7f, 0.8f, 0.5f);
            }
        }
        
        public BoostConfig GetConfig() => _config;

        public void BaseGroundBoost()
        {
            float dt = Time.deltaTime;
            var config = actor.config;
            var body = actor.kinematics.Rigidbody;
            float startForce = _config.startSpeed;
            float speed = actor.kinematics.HorizontalSpeed;
            if (Active && speed < startForce)
            {
                body.linearVelocity = body.transform.forward * startForce;
                restoringTopSpeed = true;
            }
    
            if (Active)
            {
                float maxSpeed = config.topSpeed * _config.maxSpeedMultiplier;
                if (speed < maxSpeed) body.AddForce(body.transform.forward * (_config.acceleration * dt), ForceMode.VelocityChange);
                    
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