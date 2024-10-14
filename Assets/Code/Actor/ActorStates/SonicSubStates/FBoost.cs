using System.Collections;
using System.Diagnostics;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.SonicSubStates.Boost;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

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
        
        public float turnSpeedReduction;
        public float maxSpeedMultiplier;
        public float startForce;
        public float airStartForce;
        public float boostInAirTime;
        public float airBoostTime;
        public bool canAirBoost;
        public float boostForce;
        public float magnetRadius;
        
        public bool restoringTopSpeed;
        [Range(10f, 30f)] public float restoreSpeed;

        private float _boostEnergy;
        private IBoostHandler _boostHandler;
        private Coroutine _cancelBoostCoroutine;

        private void Awake()
        {
            canAirBoost = true;
            BoostEnergy = 100;
            
            actor.input.BoostAction += BoostAction;
        }

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;

            ObjectEvents.OnObjectCollected += _ => BoostEnergy += 2;
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

                    _cancelBoostCoroutine = StartCoroutine(CancelBoost(boostInAirTime));
                }
            }
            
            if (obj is FStateAirBoost)
            {
                if (_cancelBoostCoroutine != null)
                    StopCoroutine(_cancelBoostCoroutine);
                
                _cancelBoostCoroutine = StartCoroutine(CancelBoost(airBoostTime));
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
                BoostEnergy += 4.5f * dt;
            }

            if (Active)
            {
                if (BoostEnergy > 0)
                {
                    BoostEnergy -= boostDrain * Time.deltaTime;
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

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (actor.stateMachine.CurrentState is FStateAir && !canAirBoost) return;
            if (actor.stateMachine.CurrentState is FStateStomp) return;
            if (actor.stateMachine.CurrentState is FStateSliding) return;
            
            if (CanBoost()) Active = obj.ReadValueAsButton();
            
            if (Active)
            {
                BoostEnergy -= startBoostDrain;
            }
        }

        private IEnumerator BoostDrain()
        {
            if (BoostEnergy > 10) BoostEnergy -= startBoostDrain;
            
            while (BoostEnergy > 0)
            {
                BoostEnergy -= boostDrain * Time.deltaTime;
                yield return null;
            }

            Active = false;
        }
        
        private IEnumerator CancelBoost(float time)
        {
            yield return new WaitForSeconds(time);
            Active = false;
            
            //if (boostDrainCoroutine != null) StopCoroutine(boostDrainCoroutine);
        }
    }
}