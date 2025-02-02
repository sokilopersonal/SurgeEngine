using System;
using System.Collections;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
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
        private string _hopAnimation = "HopL";
        
        public Coroutine animationDelayedCoroutine;
        private Coroutine _animationWaitCoroutine;
        private AnimationTransition _currentTransition;
        
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
            SetFloat(AnimatorParams.GroundSpeed, Mathf.Clamp(actor.kinematics.Speed, 4, 30f));
            SetFloat(AnimatorParams.VerticalSpeed, actor.stats.currentVerticalSpeed);
            
            SetFloat("SpeedPercent", Mathf.Clamp(actor.kinematics.Speed / actor.config.topSpeed, 0f, 1.25f));

            Vector3 vel = actor.kinematics.Velocity;
            float signed = Vector3.SignedAngle(vel, actor.model.root.forward, -Vector3.up);
            float angle = signed * 0.3f;

            Vector3 cross = Vector3.Cross(actor.model.root.forward, actor.kinematics.Normal);
            float mDot = Vector3.Dot(vel, cross);
            mDot = Mathf.Clamp(mDot * 0.3f, -1f, 1f);
            
            SetFloat(AnimatorParams.SmoothTurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.SmoothTurnAngle), angle, 4f * Time.deltaTime));
            SetFloat(AnimatorParams.TurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.TurnAngle), -mDot, 4f * Time.deltaTime));
            
            float dot = Vector3.Dot(Vector3.up, actor.transform.right);
            SetFloat("WallDot", -dot);
            SetFloat("AbsWallDot", Mathf.Lerp(animator.GetFloat("AbsWallDot"), 
                Mathf.Abs(Mathf.Approximately(actor.stats.groundAngle, 90) ? dot : 0), 1 * Time.deltaTime));
        }

        // TODO: Get rid of Delayed animations (switch to my transition)
        private void ChangeStateAnimation(FState obj)
        {
            FStateMachine machine = actor.stateMachine;
            FState prev = machine.PreviousState;
            
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            if (animationDelayedCoroutine != null)
                StopCoroutine(animationDelayedCoroutine);
            
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
                }
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
                        TransitionToStateDelayed("SitLoop", 0.1f, 0.673f);
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
                TransitionToState("Air Boost", 0f);
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

        public AnimationTransition TransitionToState(string stateName, float transitionTime = 0.25f)
        {
            if (_animationWaitCoroutine != null)
            {
                _currentTransition?.Cancel();
                StopCoroutine(_animationWaitCoroutine);
                _animationWaitCoroutine = null;
            }
            animator.TransitionToState(stateName, ref _currentAnimation, transitionTime);
    
            var transition = new AnimationTransition(this);
            _currentTransition = transition;
            _animationWaitCoroutine = StartCoroutine(WaitForAnimationEnd(stateName, transition));
            return transition;
        }
        
        public void TransitionToStateDelayed(string stateName, float transitionTime = 0.25f, float delay = 0.5f)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            _coroutine = StartCoroutine(TransitionDelayed(stateName, transitionTime, delay));
        }

        private IEnumerator TransitionDelayed(string stateName, float transitionTime = 0.25f, float delay = 0.5f)
        {
            yield return new WaitForSeconds(delay);
            
            animator.TransitionToState(stateName, ref _currentAnimation, transitionTime);
        }
        
        private IEnumerator WaitForAnimationEnd(string stateName, AnimationTransition transition)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (!stateInfo.IsName(stateName))
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
            while (stateInfo.normalizedTime < 1f)
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
            if (!transition.IsCanceled)
                transition.InvokeEnd();
            _animationWaitCoroutine = null;
            _currentTransition = null;
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
                    TransitionToStateDelayed(AnimatorParams.AirCycle, 0.25f, 0.5f);
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
        
        public void ResetCurrentAnimationState() => _currentAnimation = string.Empty;
        public string GetCurrentAnimationState() => _currentAnimation;
    }
    
    public class AnimationTransition
    {
        private ActorAnimation _owner;
        public AnimationTransition(ActorAnimation owner) => _owner = owner;

        public event Action OnAnimationEnd;
        public bool IsCanceled { get; private set; }
        public void Cancel()
        {
            IsCanceled = true;
        }
        internal void InvokeEnd() => OnAnimationEnd?.Invoke();

        /// <summary>
        /// Adds an action to be called when the animation ends
        /// </summary>
        /// <param name="action"></param>
        public void Then(Action action) => OnAnimationEnd += action;

        /// <summary>
        /// Adds an action to be called after a delay after the animation starts
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        public void After(float delay, Action action)
        {
            _owner.animationDelayedCoroutine = _owner.StartCoroutine(DelayedAction(delay, action));
        }

        /// <summary>
        /// Adds an action to be called after a delay after the animation ends
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        public void AfterThen(float delay, Action action)
        {
            OnAnimationEnd += () => _owner.StartCoroutine(DelayedAction(delay, action));
        }

        private IEnumerator DelayedAction(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
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