using System.Collections;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.Parameters.SonicSubStates
{
    public class FBoost : FActorSubState
    {
        public float turnSpeedReduction;
        public float maxSpeedMultiplier;
        public float startForce;
        public float airStartForce;
        public float airTime;
        public bool canAirBoost;
        public float boostForce;
        public float lastBoostTime;
        
        public bool restoringTopSpeed;
        [Range(0.2f, 3f)] public float restoreSpeed;

        private Coroutine airBoostTime;
        private Coroutine boostInAirTime;

        private void Awake()
        {
            canAirBoost = true;
            
            actor.input.BoostAction += BoostAction;
        }

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;
            OnActiveChanged += OnActiveChange;
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= OnStateAssign;
            OnActiveChanged -= OnActiveChange;
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
                    if (boostInAirTime != null) 
                        StopCoroutine(boostInAirTime);

                    boostInAirTime = StartCoroutine(BoostInAirTime());
                }
            }
            
            if (obj is FStateAirBoost)
            {
                if (airBoostTime != null)
                    StopCoroutine(airBoostTime);
                
                airBoostTime = StartCoroutine(AirBoostTime());
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (actor.stateMachine.CurrentState is FStateGround)
            {
                if (actor.input.BoostHeld)
                {
                    if (boostInAirTime != null)
                    {
                        StopCoroutine(boostInAirTime);
                    }

                    if (airBoostTime != null)
                    {
                        StopCoroutine(airBoostTime);
                    }
                }
            }
        }

        private void OnActiveChange(FSubState arg1, bool arg2)
        {
            if (arg2)
            {
                lastBoostTime = Time.time;
            }
        }

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (actor.stateMachine.CurrentState is FStateAir && !canAirBoost) return;
            
            Active = obj.ReadValueAsButton();
        }

        private IEnumerator BoostInAirTime()
        {
            yield return new WaitForSeconds(airTime);
            Active = false;
        }

        private IEnumerator AirBoostTime()
        {
            yield return new WaitForSeconds(airTime);
            Active = false;
        }
    }
}