using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorAnimation : StateAnimator, IActorComponent
    {
        public Actor actor { get; set; }
        
        public void OnInit()
        {
            actor.stateMachine.OnStateAssign += ChangeStateAnimation;
        }

        private void OnEnable()
        {
            if (actor) actor.stateMachine.OnStateAssign += ChangeStateAnimation;
        }

        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= ChangeStateAnimation;
        }

        protected override void AnimationTick()
        {
            SetBool(AnimatorParams.Idle, actor.stateMachine.currentStateName == "FStateIdle");
            SetBool(AnimatorParams.InAir, actor.stateMachine.currentStateName == "FStateAir" ||
                                          actor.stateMachine.currentStateName == "FStateJump" ||
                                          actor.stateMachine.currentStateName == "FStateSpecialJump" ||
                                          actor.stateMachine.currentStateName == "FStateAirBoost");
            SetFloat(AnimatorParams.GroundSpeed, Mathf.Clamp(actor.stats.currentSpeed, 0, 30f));
            SetFloat(AnimatorParams.VerticalSpeed, actor.stats.currentVerticalSpeed);

            Vector3 vel = actor.rigidbody.linearVelocity.normalized;
            float signed = actor.model.root.forward.SignedAngleByAxis(vel, actor.transform.up);
            float angle = signed * 0.4f;
            
            SetFloat("GrindLean", actor.kinematics.GetInputDir().x);
            
            SetFloat(AnimatorParams.SmoothTurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.SmoothTurnAngle), angle, 4f * Time.deltaTime));
            SetFloat(AnimatorParams.TurnAngle, angle);
            
            float dot = Vector3.Dot(Vector3.up, actor.transform.right);
            SetFloat("WallDot", -dot);
            SetFloat("AbsWallDot", Mathf.Lerp(animator.GetFloat("AbsWallDot"), 
                Mathf.Abs(Mathf.Approximately(actor.stats.groundAngle, 90) ? dot : 0), 1 * Time.deltaTime));
            
            SetBool("Skidding", actor.kinematics.Skidding && !actor.stateMachine.GetSubState<FBoost>().Active);
        }

        private void ChangeStateAnimation(FState obj)
        {
            var prev = actor.stateMachine.PreviousState;
            
            if (obj is FStateIdle)
            {
                if (prev is FStateGround or FStateSit)
                {
                    TransitionToState("Idle", 0.2f);
                }
                else if (prev is FStateStomp)
                {
                    TransitionToState("StompSquat", 0.1f);
                }
            }
            if (obj is FStateGround)
            {
                if (prev is not FStateDrift)
                {
                    if (prev is FStateIdle or FStateSliding)
                    {
                        TransitionToState(AnimatorParams.RunCycle, 0.2f);
                    }
                    else if (prev is FStateAir or FStateSpecialJump)
                    {
                        // TransitionToState("RestoreJog", 0f);
                        // _currentAnimation = AnimatorParams.RunCycle;

                        TransitionToState(AnimatorParams.RunCycle, 0f);
                    }
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
                }
            }
            if (obj is FStateAir && prev is not FStateSpecialJump and not FStateAirBoost and not FStateAfterHoming)
            {
                TransitionToState(AnimatorParams.AirCycle, prev switch
                {
                    FStateGround => 0.2f,
                    FStateGrindJump => 0.5f,
                    FStateJump or FStateHoming => 0f,
                    _ => 0.2f
                });
            }
            if (obj is FStateSliding)
            {
                TransitionToState("Sliding", 0.2f, true);
            }
            if (obj is FStateSit)
            {
                TransitionToState("Sit", 0.2f, true);
            }
            if (obj is FStateJump)
            {
                TransitionToState("Ball", 0f, true);
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
        }
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