using System.Collections;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.Parameters.SonicSubStates
{
    public class FBoost : FActorSubState
    {
        [Range(0, 100)] public float boostEnergy;
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

        private Coroutine cancelBoostCoroutine;

        private void Awake()
        {
            canAirBoost = true;
            
            actor.input.BoostAction += BoostAction;
        }

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;

            ActorEvents.OnRingCollected += _ => boostEnergy += 2;
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= OnStateAssign;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is FStateGround)
            {
                canAirBoost = true;
            }

            if (obj is FStateAir)
            {
                if (canAirBoost)
                {
                    if (cancelBoostCoroutine != null) 
                        StopCoroutine(cancelBoostCoroutine);

                    cancelBoostCoroutine = StartCoroutine(CancelBoost(boostInAirTime));
                }
            }
            
            if (obj is FStateAirBoost)
            {
                if (cancelBoostCoroutine != null)
                    StopCoroutine(cancelBoostCoroutine);
                
                cancelBoostCoroutine = StartCoroutine(CancelBoost(airBoostTime));
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (actor.stateMachine.CurrentState is FStateGround)
            {
                if (!actor.flags.HasFlag(FlagType.OutOfControl))
                {
                    if (actor.input.BoostHeld)
                    {
                        if (cancelBoostCoroutine != null)
                        {
                            StopCoroutine(cancelBoostCoroutine);
                        }

                        if (cancelBoostCoroutine != null)
                        {
                            StopCoroutine(cancelBoostCoroutine);
                        }
                    }
                }
            }

            if (Active)
            {
                if (boostEnergy > 0)
                {
                    boostEnergy -= boostDrain * Time.deltaTime;
                }
                else
                {
                    Active = false;
                }
            }
            
            boostEnergy = Mathf.Clamp(boostEnergy, 0, 100);
        }

        public bool CanBoost() => boostEnergy > 0;

        public bool ApplyAirForce(Rigidbody rb, Vector3 force)
        {
            if (!CanBoost()) return false;
            
            rb.linearVelocity = force;
            return true;
        }

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (actor.stateMachine.CurrentState is FStateAir && !canAirBoost) return;
            
            if (CanBoost()) Active = obj.ReadValueAsButton();
            
            if (Active)
            {
                boostEnergy -= startBoostDrain;
            }
        }

        private IEnumerator BoostDrain()
        {
            if (boostEnergy > 10) boostEnergy -= startBoostDrain;
            
            while (boostEnergy > 0)
            {
                boostEnergy -= boostDrain * Time.deltaTime;
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