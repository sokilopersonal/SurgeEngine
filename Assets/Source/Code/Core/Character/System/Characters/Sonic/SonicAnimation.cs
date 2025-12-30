using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Core.StateMachine.Components;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using Random = UnityEngine.Random;

namespace SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic
{
    public class SonicAnimation : CharacterAnimation
    {
        private string _hopAnimation = "HopL";
        private string _grindSuffix => StateAnimator.Animator.GetBool("GrindFlip") ? "_L" : "";

        protected override void ChangeAnimationState(FState obj)
        {
            base.ChangeAnimationState(obj);
            
            FStateMachine machine = Character.StateMachine;
            FState prev = machine.PreviousState;
            Animator animator = StateAnimator.Animator;

            if (obj is FStateStart)
            {
                var data = Character.GetStartData();
                AnimationHandle startHandle = null;
                if (data.startType == StartType.Standing)
                {
                    startHandle = StateAnimator.TransitionToState("StandingStart", 0f);
                }
                else if (data.startType == StartType.Prepare)
                {
                    startHandle = StateAnimator.TransitionToState("PrepareStart", 0f);
                }

                startHandle?.Then(() => StateAnimator.TransitionToState("Idle"));

                return;
            }

            if (obj is FStatePulley)
            {
                StateAnimator.TransitionToState("PulleyStart", 0f).After(1.0f, () => StateAnimator.TransitionToState("PulleyLoop", 0f));
            }

            if (obj is FStateSkydive)
            {
                StateAnimator.TransitionToState("SkydiveStart", 0f).Then(() => StateAnimator.TransitionToState("SkydiveLoop", 0f));
            }
            
            if (obj is FStateIdle)
            {
                if (prev is not FStateDamageLand)
                {
                    switch (prev)
                    {
                        case FStateGround:
                            StateAnimator.TransitionToState("Idle", 0.1f);
                            break;
                        case FStateAir:
                            float verticalSpeed = Character.Kinematics.VerticalVelocity.magnitude;
                            bool hard = verticalSpeed > 9;
                            StateAnimator.TransitionToState(!hard ? "Landing" : "LandingL", 0f).After(0.4f, () => StateAnimator.TransitionToState("Idle", 1f));
                            break;
                        case FStateSit:
                            StateAnimator.TransitionToState("SitExit", 0f).After(0.167f, () => StateAnimator.TransitionToState("Idle", 0f));
                            break;
                        case FStateStompLand:
                            StateAnimator.TransitionToStateDelayed("SitExit", 0.7f, 0.1f).Then(() =>
                            {
                                StateAnimator.TransitionToState("Idle", 0f);
                            });
                            break;
                        case FStateSweepKick:
                            StateAnimator.TransitionToStateDelayed("SitExit", 0.25f, 0.1f).Then(() => StateAnimator.TransitionToState("Idle", 0f));
                            break;
                        case FStateBrakeTurn:
                            StateAnimator.TransitionToState("Idle", 0.1f);
                            break;
                        case FStateSkydive:
                            StateAnimator.TransitionToState("SkydiveLand", 0f).Then(() => StateAnimator.TransitionToState("SkydiveLandIdle", 0f).After(0.8f, () => StateAnimator.TransitionToState("Idle", 0.2f)));
                            break;
                        default:
                            StateAnimator.TransitionToState("Idle");
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
                        StateAnimator.TransitionToState(AnimatorParams.RunCycle,
                            StateAnimator.GetCurrentAnimationState() == "Ball" ? 0f : 0.2f);
                        return;
                    }
            
                    if (machine.IsPrevExact<FStateAir>())
                    {
                        StateAnimator.TransitionToState(AnimatorParams.RunCycle);
                        return;
                    }

                    if (machine.IsPrevExact<FStateSkydive>())
                    {
                        StateAnimator.TransitionToState("SkydiveLand", 0f).After(0.067f, () => StateAnimator.TransitionToState(AnimatorParams.RunCycle));
                        return;
                    }
            
                    if (machine.IsPrevExact<FStateCrawl>())
                    {
                        StateAnimator.TransitionToState("CrawlExit", 0f).After(0.11655f, () => StateAnimator.TransitionToState(AnimatorParams.RunCycle));
                        return;
                    }
            
                    if (machine.IsPrevExact<FStateSlide>())
                    {
                        StateAnimator.TransitionToState("SlideToSit", 0f).After(0.175f, () => StateAnimator.TransitionToState(AnimatorParams.RunCycle, 0.2f));
                        return;
                    }
                    
                    if (machine.IsPrevExact<FStateSweepKick>())
                    {
                        StateAnimator.TransitionToState(AnimatorParams.RunCycle, 0.25f);
                        return;
                    }

                    if (machine.IsPrevExact<FStateQuickstep>())
                    {
                        StateAnimator.TransitionToState(AnimatorParams.RunCycle, 0.3f);
                        return;
                    }
            
                    StateAnimator.TransitionToState(AnimatorParams.RunCycle, 0.2f);
                }
                else
                {
                    // Drift exit 
                    float angle = animator.GetFloat(AnimatorParams.TurnAngle);
                    if (angle < 0f)
                    {
                        StateAnimator.TransitionToState("Drift_EL", 0.2f);
                    }
                    else if (angle > 0f)
                    {
                        StateAnimator.TransitionToState("Drift_ER", 0.2f);
                    }
            
                    StateAnimator.SetCurrentAnimationState(AnimatorParams.RunCycle);
                }
            }
            if (obj is FStateAir && prev is not FStateAfterHoming and not FStateAirBoost and not FStateTrick and not FStateDashRing)
            {
                if (prev is FStateUpreel)
                {
                    StateAnimator.TransitionToState("PulleyExit", 0.1f).AfterThen(0.25f, () => StateAnimator.TransitionToState(AnimatorParams.AirCycle, 1f));
                    return;
                }

                if (prev is FStateJumpPanel)
                {
                    StateAnimator.TransitionToStateDelayed(AnimatorParams.AirCycle, 0.5f, 1f);
                    return;
                }

                if (prev is FStateSpring)
                {
                    StateAnimator.TransitionToStateDelayed(AnimatorParams.AirCycle, 0.25f, 0.5f);
                    return;
                }
                
                StateAnimator.TransitionToState(AnimatorParams.AirCycle, prev switch
                {
                    FStateGround => 0.2f,
                    FStateGrindJump => 0.1f,
                    FStateJump or FStateHoming => 0f,
                    FStateRailSwitch => 0.5f,
                    FStateJumpSelectorLaunch => 0.3f,
                    _ => 0.2f
                });
            }
            if (obj is FStateAir && prev is FStateSpecialJump)
            {
                 StateAnimator.TransitionToState(AnimatorParams.AirCycle, 0.5f);
            }
            if (obj is FStateSlide)
            {
                StateAnimator.TransitionToState("Sliding", 0.15f);
            }
            if (obj is FStateSit)
            {
                switch (prev)
                {
                    case FStateSlide:
                        StateAnimator.TransitionToState("SlideToSit", 0f).Then(() => StateAnimator.TransitionToState("SitLoop", 0f));
                        break;
                    case FStateCrawl:
                        StateAnimator.TransitionToState("CrawlExit", 0f).After(0.333f, () => StateAnimator.TransitionToState("SitLoop", 0f));
                        break;
                    case FStateSweepKick:
                        StateAnimator.TransitionToState("SitLoop", 0.1f);
                        break;
                    case FStateStompLand:
                        StateAnimator.TransitionToStateDelayed("SitLoop", 0.1f, 0.673f);
                        break;
                    default:
                        StateAnimator.TransitionToState("SitEnter", 0f).After(0.167f, () => StateAnimator.TransitionToState("SitLoop", 0f));
                        break;
                }
            }
            if (obj is FStateCrawl)
            {
                StateAnimator.TransitionToState("CrawlEnter", 0f).Then(() => StateAnimator.TransitionToState("CrawlLoop", 0f));
            }
            if (obj is FStateSweepKick)
            {
                StateAnimator.TransitionToState("Sweepkick", 0.2f);
            }
            if (obj is FStateJump)
            {
                if (prev is FStatePulley)
                {
                    StartCoroutine(PlayPulleyJump());
                }
                else if (machine.IsPrevExact<FStateJump>())
                {
                    StateAnimator.TransitionToState("Ball", 0f);
                }
                else
                {
                    StartCoroutine(PlayHop());
                }
            }
            else
            {
                StopCoroutine(PlayHop());
            }
            if (obj is FStateGrindJump)
            {
                StateAnimator.TransitionToState("GrindJump"+_grindSuffix, 0.2f);
            }
            if (obj is FStateHoming)
            {
                StateAnimator.TransitionToState("HomingBall", 0f).After(0.2f, () => StateAnimator.TransitionToState(AnimatorParams.AirCycle, 0.15f));
            }
            if (obj is FStateStomp)
            {
                StateAnimator.TransitionToState("Stomp", 0.1f);
            }
            if (obj is FStateStompLand)
            {
                StateAnimator.TransitionToState("StompSquat", 0f);
            }
            if (obj is FStateAirBoost)
            {
                StateAnimator.TransitionToState("Air Boost", 0f).AfterThen(0.25f, () => StateAnimator.TransitionToState(AnimatorParams.AirCycle, 0.5f));
            }
            if (obj is FStateAfterHoming)
            {
                int index = Random.Range(0, 4);
                StateAnimator.TransitionToState($"AfterHoming{index}", 0f);
            }
            if (obj is FStateDrift)
            {
                // Drift start
                float angle = animator.GetFloat(AnimatorParams.TurnAngle);
                if (angle < 0)
                {
                    StateAnimator.TransitionToState("Drift_SL", 0.2f);
                }
                else if (angle > 0)
                {
                    StateAnimator.TransitionToState("Drift_SR", 0.2f);
                }
                
                StateAnimator.SetCurrentAnimationState("Drift");
            }
            if (obj is FStateJumpPanel jumpPanel)
            {
                StateAnimator.TransitionToState(!jumpPanel.IsDelux ? "Jump Standard" : "Jump Delux", 0);
            }
            if (obj is FStateTrickJump)
            {
                StateAnimator.TransitionToState("Jump Spring", 0.2f);
            }
            if (obj is FStateSpring)
            {
                StateAnimator.ResetCurrentAnimationState();
                StateAnimator.TransitionToState("Jump Spring", 0.2f);
            }
            if (obj is FStateDashRing)
            {
                StateAnimator.ResetCurrentAnimationState();
                StateAnimator.TransitionToState("Dash Ring", 0).Then(() => StateAnimator.TransitionToState(AnimatorParams.AirCycle, 0.5f));
            }
            if (obj is FStateGrind && prev is not FStateGrindSquat)
            {
                StateAnimator.TransitionToState("Grind_S" + _grindSuffix, 0f);
            }
            if (obj is FStateGrindSquat)
            {
                StateAnimator.TransitionToState("GrindSquat" + _grindSuffix, 0.25f);
            }
            if (obj is FStateGrind && prev is FStateGrindSquat)
            {
                StateAnimator.TransitionToState("GrindSwitch" + _grindSuffix, 0f).Then(() => 
                { 
                    animator.SetBool("GrindFlip", !animator.GetBool("GrindFlip")); 
                    StateAnimator.TransitionToState("GrindLoop" + _grindSuffix, 0f); 
                }); 
            }
            if (obj is FStateJumpSelector)
            {
                StateAnimator.TransitionToState("Ball", 0);
            }
            if (obj is FStateBrake)
            {
                StateAnimator.TransitionToState("BrakeCycle");
            }
            if (obj is FStateBrakeTurn)
            {
                StateAnimator.TransitionToState("BrakeTurn", 0.1f);
            }
            if (obj is FStateQuickstep quickstep)
            {
                var dir = quickstep.GetDirection();
            
                if (dir == QuickstepDirection.Left)
                {
                    string left = "QuickstepLeft";
                    if (quickstep.IsRun) left = "RunQuickstepLeft";
                    StateAnimator.TransitionToState(left, 0.1f);
                }
                else if (dir == QuickstepDirection.Right)
                {
                    string right = "QuickstepRight";
                    if (quickstep.IsRun) right = "RunQuickstepRight";
                    StateAnimator.TransitionToState(right, 0.1f);
                }
            }
            if (obj is FStateSwing)
            {
                StateAnimator.TransitionToState("SwingLoop", 0f);
            }
            if (obj is FStateSwingJump)
            {
                StateAnimator.TransitionToState("SwingJump", 0f).Then(() => { StateAnimator.TransitionToState("SwingJumpLoop", 0f); });
            }
            if (obj is FStateDamage)
            {
                StateAnimator.TransitionToState("NearDamage", 0);
            }
            if (obj is FStateDamageLand)
            {
                StateAnimator.TransitionToStateDelayed("DamageRestore", 1f, 0);
            }
            if (obj is FStateUpreel)
            {
                StateAnimator.TransitionToState("UpreelStart", 0f).Then(() => StateAnimator.TransitionToState("UpreelLoop", 0.25f));
            }
            if (obj is FStateTrick)
            {
                StateAnimator.TransitionToState($"Trick {Random.Range(1, 8)}").Then(() => StateAnimator.TransitionToState(AnimatorParams.AirCycle));
            }
            if (obj is FStateRailSwitch railSwitch)
            {
                if (railSwitch.IsLeft)
                {
                    StateAnimator.TransitionToState("RailSwitchL" + _grindSuffix, 0.0f);
                }
                else
                {
                    StateAnimator.TransitionToState("RailSwitchR" + _grindSuffix, 0.0f);
                }
            }
            if (obj is FStateStumble)
            {
                StateAnimator.TransitionToState("StumbleC");
            }
            if (obj is FStateLightSpeedDash)
            {
                StateAnimator.TransitionToState("LightSpeedDash");
            }
            if (obj is FStateReactionPlateJump)
            {
                StateAnimator.TransitionToState("Jump Standard", 0f);
            }
        }
        
        private IEnumerator PlayHop()
        {
            var actor = Character;
            bool hop = actor.Kinematics.Speed > 5;
            _hopAnimation = _hopAnimation == "HopL" ? "HopR" : "HopL";
            StateAnimator.TransitionToState(hop ? _hopAnimation : "JumpStart", 0f);
            
            yield return new WaitForSeconds(actor.Config.jumpMaxShortTime);

            if (actor.StateMachine.CurrentState is not FStateJump)
                yield break;
            
            if (actor.Input.AHeld)
            {
                StateAnimator.TransitionToState("Ball", 0f);
            }
            else
            {
                if (hop)
                {
                    StateAnimator.TransitionToStateDelayed(AnimatorParams.AirCycle, 0.277f, 0.25f);
                }
                else
                {
                    StateAnimator.TransitionToState("JumpLow").After(0.25f, () =>
                    {
                        StateAnimator.TransitionToState(AnimatorParams.AirCycle);
                    });
                }
            }
        }

        private IEnumerator PlayPulleyJump()
        {
            var actor = Character;
            StateAnimator.TransitionToState("PulleyJump", 0f);

            yield return new WaitForSeconds(0.333f);

            if (actor.StateMachine.CurrentState is not FStateJump)
                yield break;

            if (actor.Input.AHeld)
            {
                StateAnimator.TransitionToState("Ball", 0f);
            }
            else
            {
                StateAnimator.TransitionToState("Ball", 0f).Then(() => StateAnimator.TransitionToState(AnimatorParams.AirCycle, 0f));
            }
        }
    }
}