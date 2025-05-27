using System.Collections;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Core.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic
{
    public class SonicAnimation : ActorAnimation
    {
        private string _hopAnimation = "HopL";

        protected override void ChangeAnimationState(FState obj)
        {
            base.ChangeAnimationState(obj);
            
            const string Drift = "Drift";
            
            FStateMachine machine = Actor.stateMachine;
            FState prev = machine.PreviousState;
            Animator animator = StateAnimator.Animator;
            
            if (obj is FStateIdle)
            {
                string[] idleAnims = 
                {
                    "sonic_idle_A",
                    "sonic_idle_B",
                    "sonic_idle_C",
                    "sonic_idle_D",
                    "sonic_idle_E_start",
                };
                    
                int idleIndex = Random.Range(0, idleAnims.Length);
                AnimationHandle idleHandle = null;
                
                if (prev is not FStateDamageLand)
                {
                    switch (prev)
                    {
                        case FStateGround:
                            idleHandle = StateAnimator.TransitionToState("Idle", 0.1f);
                            break;
                        case FStateAir:
                            StateAnimator.TransitionToState("Landing", 0f).After(0.4f, () => StateAnimator.TransitionToState("Idle", 1f));
                            break;
                        case FStateSit:
                            StateAnimator.TransitionToState("SitExit", 0f).After(0.167f, () => StateAnimator.TransitionToState("Idle", 0f));
                            break;
                        case FStateStompLand:
                            StateAnimator.TransitionToStateDelayed("SitExit", 0.7f, 0.1f).Then(() =>
                            {
                                idleHandle = StateAnimator.TransitionToState("Idle", 0f);
                            });
                            break;
                        case FStateSweepKick:
                            StateAnimator.TransitionToStateDelayed("SitExit", 0.25f, 0.1f).Then(() => StateAnimator.TransitionToState("Idle", 0f));
                            break;
                        case FStateBrakeTurn:
                            idleHandle = StateAnimator.TransitionToState("Idle", 0.1f);
                            break;
                    }
                }

                idleHandle?.AfterThen(2f, () => StateAnimator.TransitionToState(idleAnims[idleIndex], 0.1f).Then(() => StateAnimator.TransitionToState("Idle", 0f)));
            }
            if (obj is FStateGround)
            {
                if (prev is not FStateDrift)
                {
                    if (machine.IsPrevExact<FStateJump>())
                    {
                        if (StateAnimator.GetCurrentAnimationState() == "Ball")
                            StateAnimator.TransitionToState(AnimatorParams.RunCycle, 0f);
                        else
                            StateAnimator.TransitionToState(AnimatorParams.RunCycle, 0.2f);
                        return;
                    }
            
                    if (machine.IsPrevExact<FStateAir>())
                    {
                        StateAnimator.TransitionToState(AnimatorParams.RunCycle);
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
            if (obj is FStateAir && prev is not FStateSpecialJump and not FStateAfterHoming and not FStateAirBoost)
            {
                if (prev is FStateUpreel)
                {
                    StateAnimator.TransitionToState("PulleyExit", 0.1f).AfterThen(0.25f, () => StateAnimator.TransitionToState(AnimatorParams.AirCycle, 1f));
                    return;
                }
                
                StateAnimator.TransitionToState(AnimatorParams.AirCycle, prev switch
                {
                    FStateGround => 0.2f,
                    FStateGrindJump => 0.5f,
                    FStateJump or FStateHoming => 0f,
                    _ => 0.2f
                });
            }
            if (obj is FStateAir && prev is FStateSpecialJump)
            {
                if (Actor.stateMachine.GetState<FStateSpecialJump>().data.type != SpecialJumpType.TrickJumper) StateAnimator.TransitionToState(AnimatorParams.AirCycle, 0.5f);
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
                if (machine.IsPrevExact<FStateJump>())
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
                StateAnimator.TransitionToState("GrindJump", 0.2f);
            }
            if (obj is FStateHoming)
            {
                StateAnimator.TransitionToState("BallOnce", 0f).Then(() => StateAnimator.TransitionToState(AnimatorParams.AirCycle, 0.15f));
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
                
                StateAnimator.SetCurrentAnimationState(Drift);
            }
            
            if (obj is FStateGrind && prev is not FStateGrindSquat)
            {
                StateAnimator.TransitionToState("Grind_S", 0f);
            }
            
            if (obj is FStateGrindSquat)
            {
                StateAnimator.TransitionToState("GrindSquat", 0.25f);
            }
            
            if (obj is FStateGrind && prev is FStateGrindSquat)
            {
                StateAnimator.TransitionToState("GrindLoop", 0.25f);
            }
            if (obj is FStateJumpSelectorLaunch)
            {
                
            }
            if (obj is FStateBrake)
            {
                StateAnimator.TransitionToState("BrakeCycle");
            }
            if (obj is FStateBrakeTurn)
            {
                StateAnimator.TransitionToState("BrakeTurn", 0.1f);
            }
            if (obj is FStateRunQuickstep runQuickstep)
            {
                var dir = runQuickstep.GetDirection();
                
                if (dir == QuickstepDirection.Left)
                {
                    StateAnimator.TransitionToState("RunQuickstepLeft", 0.1f);
                }
                else if (dir == QuickstepDirection.Right)
                {
                    StateAnimator.TransitionToState("RunQuickstepRight", 0.1f);
                }
            }
            else if (obj is FStateQuickstep quickstep)
            {
                var dir = quickstep.GetDirection();
            
                if (dir == QuickstepDirection.Left)
                {
                    StateAnimator.TransitionToState("QuickstepLeft", 0.1f);
                }
                else if (dir == QuickstepDirection.Right)
                {
                    StateAnimator.TransitionToState("QuickstepRight", 0.1f);
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
                StateAnimator.TransitionToState("UpreelStart", 0f).Then(() => StateAnimator.TransitionToState("PulleyLoop", 0.25f));
            }
        }
        
        private IEnumerator PlayHop()
        {
            bool hop = Actor.kinematics.HorizontalSpeed > 5;
            _hopAnimation = _hopAnimation == "HopL" ? "HopR" : "HopL";
            StateAnimator.TransitionToState(hop ? _hopAnimation : "JumpStart", 0f);
            
            yield return new WaitForSeconds(0.117f);

            if (Actor.stateMachine.CurrentState is not FStateJump)
                yield break;
            
            if (Actor.input.JumpHeld)
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
                    StateAnimator.TransitionToState("JumpLow", 0.25f).After(0.25f, () =>
                    {
                        StateAnimator.TransitionToState(AnimatorParams.AirCycle, 0.25f);
                    });
                }
            }
        }
    }
}