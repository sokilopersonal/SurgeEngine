using System.Collections;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Misc;
using SurgeEngine.Code.SonicSubStates.Boost;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.Parameters.SonicSubStates
{
    public class FBoost : FActorSubState
    {
        public float BoostEnergy
        {
            get => _boostEnergy;
            private set => _boostEnergy = Mathf.Clamp(value, 0, maxBoostEnergy);
        }
        [Range(45, 100)] public float maxBoostEnergy;
        [SerializeField] private float boostDrain = 3;
        [SerializeField] private float startBoostDrain = 10;
        
        public bool canAirBoost;
        
        public bool restoringTopSpeed;
        [Range(10f, 30f)] public float restoreSpeed;

        private float _boostEnergy;
        private IBoostHandler _boostHandler;
        private Coroutine _cancelBoostCoroutine;
        
        private ParameterGroup _boostEnergyGroup;

        private void Awake()
        {
            canAirBoost = true;
            BoostEnergy = 100;
            
            _boostEnergyGroup = SonicGameDocument.Instance.GetDocument("Sonic").GetGroup("BoostEnergy");
            
            actor.input.BoostAction += BoostAction;
        }

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;

            ObjectEvents.OnObjectCollected += _ => BoostEnergy += 
                _boostEnergyGroup.GetParameter<float>("BoostEnergyRingAdd");
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= OnStateAssign;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj.TryGetComponent(out IBoostHandler handler))
            {
                _boostHandler = handler;
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
                        StopCoroutine(_cancelBoostCoroutine);

                    _cancelBoostCoroutine = StartCoroutine(CancelBoost(_boostEnergyGroup.GetParameter<float>("BoostInAirTime")));
                }
            }
            
            if (obj is FStateAirBoost)
            {
                if (_cancelBoostCoroutine != null)
                    StopCoroutine(_cancelBoostCoroutine);
                
                _cancelBoostCoroutine = StartCoroutine(CancelBoost(_boostEnergyGroup.GetParameter<float>("AirBoostTime")));
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
                            StopCoroutine(_cancelBoostCoroutine);
                        }

                        if (_cancelBoostCoroutine != null)
                        {
                            StopCoroutine(_cancelBoostCoroutine);
                        }
                    }
                }
            }

            if (actor.stateMachine.CurrentState is FStateDrift)
            {
                BoostEnergy += _boostEnergyGroup.GetParameter<float>("BoostEnergyDriftAdd") * dt;
            }

            if (Active)
            {
                if (BoostEnergy > 0)
                {
                    BoostEnergy -= _boostEnergyGroup.GetParameter<float>("BoostEnergyDrain") * Time.deltaTime;
                }
                else
                {
                    Active = false;
                }
            }

            if (Common.CheckForGround(out _, CheckGroundType.Predict))
            {
                Active = false;
            }
            
            BoostEnergy = Mathf.Clamp(BoostEnergy, 0, 100);
        }

        public bool CanBoost() => BoostEnergy > 0;

        public bool ApplyAirForce(Rigidbody rb, Vector3 force)
        {
            if (!CanBoost()) return false;
            
            rb.linearVelocity = force;
            return true;
        }
        
        public ParameterGroup GetBoostEnergyGroup() => _boostEnergyGroup;

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (actor.stateMachine.CurrentState is FStateAir && !canAirBoost) return;
            if (actor.stateMachine.CurrentState is FStateStomp) return;
            if (actor.stateMachine.CurrentState is FStateSliding) return;
            
            if (CanBoost())
            {
                if (obj.started)
                    Active = true;
                else
                {
                    Active = false;
                }
            }
            
            if (Active)
            {
                BoostEnergy -= startBoostDrain;

                new Rumble().Vibrate(0.7f, 0.8f, 0.5f);
            }
        }
        
        private IEnumerator CancelBoost(float time)
        {
            yield return new WaitForSeconds(time);
            Active = false;
            
            //if (boostDrainCoroutine != null) StopCoroutine(boostDrainCoroutine);
        }
    }
}