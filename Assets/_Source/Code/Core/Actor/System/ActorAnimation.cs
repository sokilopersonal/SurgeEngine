using NaughtyAttributes;
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
        [field: SerializeField, Required("State Animator is required!")] public StateAnimator StateAnimator { get; private set; }

        private void OnEnable()
        {
            Actor.StateMachine.OnStateAssign += ChangeAnimationState;
        }

        private void OnDisable()
        {
            Actor.StateMachine.OnStateAssign -= ChangeAnimationState;
        }

        protected virtual void Update()
        {
            var animator = StateAnimator.Animator;
            animator.SetFloat(AnimatorParams.GroundSpeed, Mathf.Clamp(Actor.Kinematics.Speed, 0, 30f));
            animator.SetFloat(AnimatorParams.VerticalSpeed, Actor.Kinematics.Velocity.y);
            
            float targetSpeedPercent = Mathf.Clamp(Actor.Kinematics.Speed / Actor.Config.topSpeed, 0.02f, 1.1f);
            float currentSpeedPercent = animator.GetFloat("SpeedPercent");
            animator.SetFloat("SpeedPercent", Mathf.Lerp(currentSpeedPercent, targetSpeedPercent, 10f * Time.deltaTime));

            Vector3 vel = Actor.Kinematics.Velocity;
            float signed = Vector3.SignedAngle(vel, Actor.Model.root.forward, -Vector3.up);
            float angle = signed * 0.3f;

            Vector3 cross = Vector3.Cross(Actor.Model.root.forward, Actor.Kinematics.Normal);
            float mDot = Vector3.Dot(vel, cross);
            mDot = Mathf.Clamp(mDot * 0.3f, -1f, 1f);
            
            animator.SetFloat(AnimatorParams.SmoothTurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.SmoothTurnAngle), angle, 4f * Time.deltaTime));
            animator.SetFloat(AnimatorParams.TurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.TurnAngle), -mDot, 4f * Time.deltaTime));
            
            float dot = Vector3.Dot(Vector3.up, Actor.transform.right);
            animator.SetFloat("WallDot", -dot);
            animator.SetFloat("AbsWallDot", Mathf.Lerp(animator.GetFloat("AbsWallDot"), 
                Mathf.Abs(Mathf.Approximately(Actor.Kinematics.Angle, 90) ? dot : 0), 1 * Time.deltaTime));
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
        public static readonly int IdleTrigger = Animator.StringToHash("Idle");
        public static readonly int IdleIndex = Animator.StringToHash("IdleIndex");
        public static readonly int GroundSpeed = Animator.StringToHash("GroundSpeed");
        public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
        public static readonly int TurnAngle = Animator.StringToHash("LocalTurnAngle");
        public static readonly int SmoothTurnAngle = Animator.StringToHash("SmoothTurnAngle");
        public static readonly string RunCycle = "Run Cycle";
        public static readonly string AirCycle = "Air Cycle";
    }
}