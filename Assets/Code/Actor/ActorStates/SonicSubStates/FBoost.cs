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
        public float boostInAirTime;
        public float airBoostTime;
        public bool canAirBoost;
        public float boostForce;
        
        public bool restoringTopSpeed;
        [Range(10f, 30f)] public float restoreSpeed;

        private Coroutine airBoostTimeCoroutine;
        private Coroutine boostInAirTimeCoroutine;

        private void Awake()
        {
            canAirBoost = true;
            
            actor.input.BoostAction += BoostAction;
        }

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;
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
                    if (boostInAirTimeCoroutine != null) 
                        StopCoroutine(boostInAirTimeCoroutine);

                    boostInAirTimeCoroutine = StartCoroutine(BoostInAirTime());
                }
            }
            
            if (obj is FStateAirBoost)
            {
                if (airBoostTimeCoroutine != null)
                    StopCoroutine(airBoostTimeCoroutine);
                
                airBoostTimeCoroutine = StartCoroutine(AirBoostTime());
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (actor.stateMachine.CurrentState is FStateGround)
            {
                if (actor.input.BoostHeld)
                {
                    if (boostInAirTimeCoroutine != null)
                    {
                        StopCoroutine(boostInAirTimeCoroutine);
                    }

                    if (airBoostTimeCoroutine != null)
                    {
                        StopCoroutine(airBoostTimeCoroutine);
                    }
                }
            }
        }

        private void BoostAction(InputAction.CallbackContext obj)
        {
            if (actor.stateMachine.CurrentState is FStateAir && !canAirBoost) return;
            
            Active = obj.ReadValueAsButton();
        }

        private IEnumerator BoostInAirTime()
        {
            yield return new WaitForSeconds(boostInAirTime);
            Active = false;
        }

        private IEnumerator AirBoostTime()
        {
            yield return new WaitForSeconds(airBoostTime);
            Active = false;
            Debug.Log("hey");
        }
    }
}