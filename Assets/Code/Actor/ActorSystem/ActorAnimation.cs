using System;
using System.Collections;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorAnimation : ActorComponent
    {
        public Animator animator;

        private string _currentAnimation;
        
        private Coroutine _coroutine;

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += ChangeStateAnimation;
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= ChangeStateAnimation;
        }

        private void Update()
        {
            SetBool(AnimatorParams.Idle, actor.stateMachine.CurrentState is FStateIdle);
            SetBool(AnimatorParams.InAir, actor.stateMachine.CurrentState is FStateAir or FStateJump or FStateSpecialJump or FStateAirBoost);
            SetFloat(AnimatorParams.GroundSpeed, Mathf.Clamp(actor.stats.currentSpeed, 4, 30f));
            SetFloat(AnimatorParams.VerticalSpeed, actor.stats.currentVerticalSpeed);
            
            SetFloat("SpeedPercent", Mathf.Clamp01(actor.kinematics.HorizontalSpeed / actor.config.topSpeed));

            Vector3 vel = actor.kinematics.Velocity.normalized;
            float signed = actor.model.root.forward.SignedAngleByAxis(vel, actor.transform.up);
            float angle = signed * 0.4f;
            
            SetFloat("GrindLean", actor.kinematics.GetInputDir().x);
            
            SetFloat(AnimatorParams.SmoothTurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.SmoothTurnAngle), angle, 4f * Time.deltaTime));
            SetFloat(AnimatorParams.TurnAngle, angle);
            
            float dot = Vector3.Dot(Vector3.up, actor.transform.right);
            SetFloat("WallDot", -dot);
            SetFloat("AbsWallDot", Mathf.Lerp(animator.GetFloat("AbsWallDot"), 
                Mathf.Abs(Mathf.Approximately(actor.stats.groundAngle, 90) ? dot : 0), 1 * Time.deltaTime));
            
            SetBool("Skidding", actor.kinematics.Skidding && !SonicTools.IsBoost());
        }

        private void ChangeStateAnimation(FState obj)
        {
            FStateMachine machine = actor.stateMachine;
            FState prev = machine.PreviousState;
            
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            if (obj is FStateIdle)
            {
                if (prev is not FStateStart)
                {
                    switch (prev)
                    {
                        case FStateGround:
                            if (actor.kinematics.HorizontalSpeed >= 0.2f)
                            {
                                TransitionToState("MoveStop", 0.1f);
                                TransitionToStateDelayed("Idle", 0.2f, 0.15f);
                            }
                            else
                            {
                                TransitionToState("Idle", 0.1f);
                            }
                            break;
                        case FStateStomp:
                            TransitionToState("StompSquat", 0.1f);
                            break;
                        case FStateAir:
                            TransitionToState("Landing", 0f);
                            TransitionToStateDelayed("Idle", 1f, 0.4f);
                            break;
                    }
                }
            }
            if (obj is FStateGround)
            {
                if (prev is not FStateDrift)
                {
                    if (machine.IsPrevExact<FStateAir>())
                    {
                        TransitionToState(AnimatorParams.RunCycle, 0.25f);
                        return;
                    }
                    
                    if (machine.IsPrevExact<FStateJump>())
                    {
                        TransitionToState(AnimatorParams.RunCycle, 0f);
                        return;
                    }

                    TransitionToState(AnimatorParams.RunCycle, 0.2f);
                }
                else
                {
                    // Drift exit 
                    
                    float angle = animator.GetFloat(AnimatorParams.TurnAngle);
                    if (angle < 0f)
                    {
                        TransitionToState("Drift_EL", 0.2f);
                    }
                    else if (angle > 0f)
                    {
                        TransitionToState("Drift_ER", 0.2f);
                    }

                    _currentAnimation = AnimatorParams.RunCycle;
                }
            }
            if (obj is FStateAir && prev is not FStateAirBoost and not FStateAfterHoming)
            {
                if (prev is not FStateSpecialJump)
                {
                    TransitionToState(AnimatorParams.AirCycle, prev switch
                    {
                        FStateGround => 0.2f,
                        FStateGrindJump => 0.5f,
                        FStateJump or FStateHoming => 0f,
                        _ => 0.2f
                    });
                }
                else
                {
                    SpecialJumpData data = actor.stateMachine.GetState<FStateSpecialJump>().data;
                    if (data.type == SpecialJumpType.Spring || data.type == SpecialJumpType.DashRing) TransitionToStateDelayed(AnimatorParams.AirCycle, 0.5f, 0.4f);
                    else if (data.type == SpecialJumpType.TrickJumper)
                    {
                        TransitionToStateDelayed(AnimatorParams.AirCycle, 0.35f, 1.2f);
                    }
                }
            }
            if (obj is FStateSlide)
            {
                TransitionToState("Sliding", 0.2f, true);
            }
            if (obj is FStateSit)
            {
                TransitionToState("Sit", 0.2f, true);
            }
            if (obj is FStateJump)
            {
                TransitionToState("Ball", 0f);
            }
            if (obj is FStateGrindJump)
            {
                TransitionToState("GrindJump", 0.2f, true);
            }
            if (obj is FStateHoming)
            {
                TransitionToState("Ball", 0f, true);
            }
            if (obj is FStateStomp)
            {
                TransitionToState("Stomp", 0.1f, true);
            }
            if (obj is FStateAirBoost)
            {
                TransitionToState("Air Boost", 0f, true);
            }
            if (obj is FStateAfterHoming && prev is not FStateJump)
            {
                int index = Random.Range(0, 4);
                TransitionToState($"AfterHoming{index}", 0f);
            }

            if (obj is FStateDrift)
            {
                // Drift start
                
                float angle = animator.GetFloat(AnimatorParams.TurnAngle);
                if (angle < 0)
                {
                    TransitionToState("Drift_SL", 0.2f, true);
                }
                else if (angle > 0)
                {
                    TransitionToState("Drift_SR", 0.2f, true);
                }
                
                _currentAnimation = AnimatorParams.Drift;
            }

            if (obj is FStateGrind && prev is not FStateGrindSquat)
            {
                TransitionToState("Grind_S", 0f, true);
            }

            if (obj is FStateGrindSquat)
            {
                TransitionToState("GrindSquat", 0.25f, true);
            }

            if (obj is FStateGrind && prev is FStateGrindSquat)
            {
                TransitionToState("GrindLoop", 0.25f, true);
            }

            if (obj is FStateJumpSelectorLaunch)
            {
                
            }
        }
        
        protected void SetFloat(string state, float value)
        {
            animator.SetFloat(state, value);
        }

        protected void SetBool(string state, bool value)
        {
            animator.SetBool(state, value);
        }

        protected void SetFloat(int id, float value)
        {
            animator.SetFloat(id, value);
        }

        protected void SetBool(int id, bool value)
        {
            animator.SetBool(id, value);
        }

        protected void SetAction(bool value)
        {
            SetBool("InAction", value);
        }

        public void ResetAction()
        {
            SetAction(false);
        }

        public void TransitionToState(string stateName, float transitionTime = 0.25f, bool isAction = false)
        {
            animator.TransitionToState(stateName, ref _currentAnimation, transitionTime);

            if (isAction) SetAction(true);
        }
        
        public void TransitionToStateDelayed(string stateName, float transitionTime = 0.25f, float delay = 0.5f, bool isAction = false)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            _coroutine = StartCoroutine(TransitionDelayed(stateName, transitionTime, delay, isAction));
            
            if (isAction) SetAction(true);
        }

        private IEnumerator TransitionDelayed(string stateName, float transitionTime = 0.25f, float delay = 0.5f, bool isAction = false)
        {
            yield return new WaitForSeconds(delay);
            
            animator.TransitionToState(stateName, ref _currentAnimation, transitionTime);
        }
        
        public void ResetCurrentAnimationState() => _currentAnimation = string.Empty;
        public string GetCurrentAnimationState() => _currentAnimation;
    }

    public static class AnimatorParams
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int InAir = Animator.StringToHash("InAir");
        public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
        public static readonly int GroundSpeed = Animator.StringToHash("GroundSpeed");
        public static readonly int TurnAngle = Animator.StringToHash("TurnAngle");
        public static readonly int SmoothTurnAngle = Animator.StringToHash("SmoothTurnAngle");
        public static readonly string RunCycle = "Run Cycle";
        public static readonly string AirCycle = "Air Cycle";
        public static readonly string Drift = "Drift";
    }
}