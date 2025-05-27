using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Core.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    /// <summary>
    /// Base class for handling actor animations using Unity's animation system.
    /// All animation transitions must be implemented manually for each character.
    /// </summary>
    public abstract class ActorAnimation : ActorComponent
    {
        public StateAnimator StateAnimator { get; private set; }
        
        private void Awake()
        {
            StateAnimator = GetComponent<StateAnimator>();
        }

        private void OnEnable()
        {
            Actor.stateMachine.OnStateEarlyAssign += ChangeAnimationState;
        }

        private void OnDisable()
        {
            Actor.stateMachine.OnStateEarlyAssign -= ChangeAnimationState;
        }

        protected virtual void Update()
        {
            var animator = StateAnimator.Animator;
            animator.SetFloat(AnimatorParams.GroundSpeed, Mathf.Clamp(Actor.kinematics.Speed, 4, 30f));
            animator.SetFloat(AnimatorParams.VerticalSpeed, Actor.kinematics.Velocity.y);
            animator.SetFloat("SpeedPercent", Mathf.Clamp(Actor.kinematics.Speed / Actor.config.topSpeed, 0f, 1.25f));

            Vector3 vel = Actor.kinematics.Velocity;
            float signed = Vector3.SignedAngle(vel, Actor.model.root.forward, -Vector3.up);
            float angle = signed * 0.3f;

            Vector3 cross = Vector3.Cross(Actor.model.root.forward, Actor.kinematics.Normal);
            float mDot = Vector3.Dot(vel, cross);
            mDot = Mathf.Clamp(mDot * 0.3f, -1f, 1f);
            
            animator.SetFloat(AnimatorParams.SmoothTurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.SmoothTurnAngle), angle, 4f * Time.deltaTime));
            animator.SetFloat(AnimatorParams.TurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.TurnAngle), -mDot, 4f * Time.deltaTime));
            
            float dot = Vector3.Dot(Vector3.up, Actor.transform.right);
            animator.SetFloat("WallDot", -dot);
            animator.SetFloat("AbsWallDot", Mathf.Lerp(animator.GetFloat("AbsWallDot"), 
                Mathf.Abs(Mathf.Approximately(Actor.kinematics.Angle, 90) ? dot : 0), 1 * Time.deltaTime));
        }

        /// <summary>
        /// Calls when actor changed the current state.
        /// </summary>
        /// <param name="obj">Actor current state</param>
        protected virtual void ChangeAnimationState(FState obj)
        {
            StateAnimator.Stop();
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
    }
}