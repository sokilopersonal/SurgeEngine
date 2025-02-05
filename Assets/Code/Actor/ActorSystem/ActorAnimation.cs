using System;
using System.Collections;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.StateMachine.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorAnimation : StateAnimator
    {
        private string _hopAnimation = "HopL";
        
        private Actor actor => ActorContext.Context;

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
            animator.SetFloat(AnimatorParams.GroundSpeed, Mathf.Clamp(actor.kinematics.Speed, 4, 30f));
            animator.SetFloat(AnimatorParams.VerticalSpeed, actor.stats.currentVerticalSpeed);
            
            animator.SetFloat("SpeedPercent", Mathf.Clamp(actor.kinematics.Speed / actor.config.topSpeed, 0f, 1.25f));

            Vector3 vel = actor.kinematics.Velocity;
            float signed = Vector3.SignedAngle(vel, actor.model.root.forward, -Vector3.up);
            float angle = signed * 0.3f;

            Vector3 cross = Vector3.Cross(actor.model.root.forward, actor.kinematics.Normal);
            float mDot = Vector3.Dot(vel, cross);
            mDot = Mathf.Clamp(mDot * 0.3f, -1f, 1f);
            
            animator.SetFloat(AnimatorParams.SmoothTurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.SmoothTurnAngle), angle, 4f * Time.deltaTime));
            animator.SetFloat(AnimatorParams.TurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.TurnAngle), -mDot, 4f * Time.deltaTime));
            
            float dot = Vector3.Dot(Vector3.up, actor.transform.right);
            animator.SetFloat("WallDot", -dot);
            animator.SetFloat("AbsWallDot", Mathf.Lerp(animator.GetFloat("AbsWallDot"), 
                Mathf.Abs(Mathf.Approximately(actor.stats.groundAngle, 90) ? dot : 0), 1 * Time.deltaTime));
        }

        protected override void AnimationTick()
        {
            
        }

        protected override void ChangeStateAnimation(FState obj)
        {
            base.ChangeStateAnimation(obj);
            
            FStateMachine machine = actor.stateMachine;
            FState prev = machine.PreviousState;
            
            if (obj is FStateIdle)
            {
                if (prev is not FStateStart)
                {
                    switch (prev)
                    {
                        case FStateGround:
                            if (actor.kinematics.HorizontalSpeed >= 0.1f)
                            {
                                TransitionToState("MoveStop", 0.1f).After(0.15f, () => TransitionToState("Idle", 0.2f));
                            }
                            else
                            {
                                TransitionToState("Idle", 0.1f);
                            }
                            break;
                        case FStateAir:
                            TransitionToState("Landing", 0f).After(0.4f, () => TransitionToState("Idle", 1f));
                            break;
                        case FStateSit:
                            TransitionToState("SitExit", 0f).After(0.167f, () => TransitionToState("Idle", 0f));
                            break;
                        case FStateStompLand: // TODO: Fix the fast transition
                            TransitionToState("SitExit", 0f).After(0.167f, () => TransitionToState("Idle", 0f));
                            break;
                        case FStateSweepKick: // Here too
                            TransitionToState("SitExit", 0f).After(0.167f, () => TransitionToState("Idle", 0f));
                            break;
                        case FStateBrakeTurn:
                            TransitionToState("Idle", 0.1f);
                            break;
                    }
                }
            }
            if (obj is FStateGround)
            {
                if (prev is not FStateDrift)
                {
                    if (machine.IsPrevExact<FStateJump>())
                    {
                        if (GetCurrentAnimationState() == "Ball")
                            TransitionToState(AnimatorParams.RunCycle, 0f);
                        else
                            TransitionToState(AnimatorParams.RunCycle, 0.2f);
                        return;
                    }

                    if (machine.IsPrevExact<FStateAir>())
                    {
                        TransitionToState(AnimatorParams.RunCycle);
                        return;
                    }

                    if (machine.IsPrevExact<FStateCrawl>())
                    {
                        TransitionToState("CrawlExit", 0f).After(0.11655f, () => TransitionToState(AnimatorParams.RunCycle));
                        return;
                    }

                    if (machine.IsPrevExact<FStateSlide>())
                    {
                        TransitionToState("SlideToSit", 0f).After(0.175f, () => TransitionToState(AnimatorParams.RunCycle, 0.2f));
                        return;
                    }

                    if (machine.IsPrevExact<FStateSweepKick>())
                    {
                        TransitionToState(AnimatorParams.RunCycle, 0.25f);
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
            if (obj is FStateAir && prev is not FStateSpecialJump and not FStateAfterHoming and not FStateAirBoost)
            {
                TransitionToState(AnimatorParams.AirCycle, prev switch
                {
                    FStateGround => 0.2f,
                    FStateGrindJump => 0.5f,
                    FStateJump or FStateHoming => 0f,
                    _ => 0.2f
                });
            }
            if (obj is FStateAir && prev is FStateSpecialJump)
            {
                if (actor.stateMachine.GetState<FStateSpecialJump>().data.type != SpecialJumpType.TrickJumper) TransitionToState(AnimatorParams.AirCycle, 0.5f);
            }
            if (obj is FStateSlide)
            {
                TransitionToState("Sliding", 0.15f);
            }
            if (obj is FStateSit)
            {
                switch (prev)
                {
                    case FStateSlide:
                        TransitionToState("SlideToSit", 0f).Then(() => TransitionToState("SitLoop", 0f));
                        break;
                    case FStateCrawl:
                        TransitionToState("CrawlExit", 0f).After(0.333f, () => TransitionToState("SitLoop", 0f));
                        break;
                    case FStateSweepKick:
                        TransitionToState("SitLoop", 0.1f);
                        break;
                    case FStateStompLand:
                        //TransitionToStateDelayed("SitLoop", 0.1f, 0.673f);
                        break;
                    default:
                        TransitionToState("SitEnter", 0f).After(0.167f, () => TransitionToState("SitLoop", 0f));
                        break;
                }
            }
            if (obj is FStateCrawl)
            {
                TransitionToState("CrawlEnter", 0f).Then(() => TransitionToState("CrawlLoop", 0f));
            }
            if (obj is FStateSweepKick)
            {
                TransitionToState("Sweepkick", 0.2f);
            }
            if (obj is FStateJump)
            {
                if (machine.IsPrevExact<FStateJump>())
                    TransitionToState("Ball", 0f);
                else
                    StartCoroutine(PlayHop());
            }
            else
            {
                StopCoroutine(PlayHop());
            }
            if (obj is FStateGrindJump)
            {
                TransitionToState("GrindJump", 0.2f);
            }
            if (obj is FStateHoming)
            {
                TransitionToState("BallOnce", 0f).Then(() => TransitionToState(AnimatorParams.AirCycle, 0.15f));
            }
            if (obj is FStateStomp)
            {
                TransitionToState("Stomp", 0.1f);
            }
            if (obj is FStateStompLand)
            {
                TransitionToState("StompSquat", 0f);
            }
            if (obj is FStateAirBoost)
            {
                TransitionToState("Air Boost", 0f).AfterThen(0.25f, () => TransitionToState(AnimatorParams.AirCycle, 0.5f));
            }
            if (obj is FStateAfterHoming)
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
                    TransitionToState("Drift_SL", 0.2f);
                }
                else if (angle > 0)
                {
                    TransitionToState("Drift_SR", 0.2f);
                }
                
                _currentAnimation = AnimatorParams.Drift;
            }

            if (obj is FStateGrind && prev is not FStateGrindSquat)
            {
                TransitionToState("Grind_S", 0f);
            }

            if (obj is FStateGrindSquat)
            {
                TransitionToState("GrindSquat", 0.25f);
            }

            if (obj is FStateGrind && prev is FStateGrindSquat)
            {
                TransitionToState("GrindLoop", 0.25f);
            }

            if (obj is FStateJumpSelectorLaunch)
            {
                
            }

            if (obj is FStateBrake)
            {
                TransitionToState("BrakeCycle");
            }
            if (obj is FStateBrakeTurn)
            {
                TransitionToState("BrakeTurn", 0.1f);
            }

            if (obj is FStateRunQuickstep runQuickstep)
            {
                var dir = runQuickstep.GetDirection();
                
                if (dir == QuickstepDirection.Left)
                {
                    TransitionToState("RunQuickstepLeft", 0.1f);
                }
                else if (dir == QuickstepDirection.Right)
                {
                    TransitionToState("RunQuickstepRight", 0.1f);
                }
            }
            else if (obj is FStateQuickstep quickstep)
            {
                var dir = quickstep.GetDirection();

                if (dir == QuickstepDirection.Left)
                {
                    TransitionToState("QuickstepLeft", 0.1f);
                }
                else if (dir == QuickstepDirection.Right)
                {
                    TransitionToState("QuickstepRight", 0.1f);
                }
            }

            if (obj is FStateSwing)
            {
                TransitionToState("SwingLoop", 0f);
            }

            if (obj is FStateSwingJump)
            {
                TransitionToState("SwingJump", 0f).Then(() => { TransitionToState("SwingJumpLoop", 0f); });
            }
        }

        private IEnumerator TransitionDelayed(string stateName, float transitionTime = 0.25f, float delay = 0.5f)
        {
            yield return new WaitForSeconds(delay);
            
            animator.TransitionToState(stateName, ref _currentAnimation, transitionTime);
        }
        
        private IEnumerator PlayHop()
        {
            bool hop = actor.kinematics.HorizontalSpeed > 5;
            _hopAnimation = _hopAnimation == "HopL" ? "HopR" : "HopL";
            TransitionToState(hop ? _hopAnimation : "JumpStart", 0f);
            
            yield return new WaitForSeconds(0.117f);

            if (!(actor.stateMachine.CurrentState is FStateJump))
                yield break;
            
            if (actor.input.JumpHeld)
            {
                TransitionToState("Ball", 0f);
            }
            else
            {
                if (hop)
                {
                    TransitionToState(AnimatorParams.AirCycle, 0.8f);
                }
                else
                {
                    TransitionToState("JumpLow", 0.25f).After(0.25f, () =>
                    {
                        TransitionToState(AnimatorParams.AirCycle, 0.25f);
                    });
                }
            }
        }
    }
    
    public static class AnimatorParams
    {
        public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
        public static readonly int GroundSpeed = Animator.StringToHash("GroundSpeed");
        public static readonly int TurnAngle = Animator.StringToHash("LocalTurnAngle");
        public static readonly int SmoothTurnAngle = Animator.StringToHash("SmoothTurnAngle");
        public static readonly string RunCycle = "Run Cycle";
        public static readonly string AirCycle = "Air Cycle";
        public static readonly string Drift = "Drift";
    }
}