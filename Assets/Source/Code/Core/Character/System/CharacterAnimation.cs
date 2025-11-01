using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Core.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    /// <summary>
    /// Base class for handling actor animations using Unity's animation system.
    /// All animation transitions must be implemented manually for each character.
    /// </summary>
    [RequireComponent(typeof(StateAnimator))]
    public abstract class CharacterAnimation : CharacterComponent
    {
        public StateAnimator StateAnimator { get; private set; }

        [SerializeField] private float idleTime = 3f;
        [SerializeField] private string[] idleStates;
        private float _idleTimer;

        private void Awake()
        {
            StateAnimator = GetComponent<StateAnimator>();
        }

        private void OnEnable()
        {
            character.StateMachine.OnStateEarlyAssign += ChangeAnimationState;
        }

        private void OnDisable()
        {
            character.StateMachine.OnStateEarlyAssign -= ChangeAnimationState;
        }

        protected virtual void Update()
        {
            var animator = StateAnimator.Animator;
            animator.SetFloat(AnimatorParams.GroundSpeed, Mathf.Clamp(character.Kinematics.Speed, 0, 30f));
            animator.SetFloat(AnimatorParams.VerticalSpeed, character.Kinematics.Velocity.y);
            
            float targetSpeedPercent = Mathf.Clamp(character.Kinematics.Speed / character.Config.topSpeed, 0.02f, 1.1f);
            float currentSpeedPercent = animator.GetFloat("SpeedPercent");
            animator.SetFloat("SpeedPercent", Mathf.Lerp(currentSpeedPercent, targetSpeedPercent, 10f * Time.deltaTime));

            Vector3 vel = character.Kinematics.Velocity;
            float signed = Vector3.SignedAngle(vel, character.Model.Root.forward, -Vector3.up);
            float angle = signed * 0.3f;

            Vector3 cross = Vector3.Cross(character.Model.Root.forward, character.Kinematics.Normal);
            float mDot = Vector3.Dot(vel, cross);
            mDot = Mathf.Clamp(mDot * 0.3f, -1f, 1f);
            
            animator.SetFloat(AnimatorParams.SmoothTurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.SmoothTurnAngle), angle, 4f * Time.deltaTime));
            animator.SetFloat(AnimatorParams.TurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.TurnAngle), -mDot, 4f * Time.deltaTime));
            
            float dot = Vector3.Dot(Vector3.up, character.transform.right);
            animator.SetFloat("WallDot", -dot);
            animator.SetFloat("AbsWallDot", Mathf.Lerp(animator.GetFloat("AbsWallDot"), 
                Mathf.Abs(Mathf.Approximately(character.Kinematics.Angle, 90) ? dot : 0), 1 * Time.deltaTime));

            CalculateIdleState();
        }

        private void CalculateIdleState()
        {
            if (StateAnimator.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                _idleTimer += Time.deltaTime;
                if (_idleTimer >= idleTime)
                {
                    int index = Random.Range(0, idleStates.Length);
                    StateAnimator.Animator.SetInteger(AnimatorParams.IdleIndex, index);
                    StateAnimator.Animator.SetTrigger(AnimatorParams.IdleTrigger);
                    StateAnimator.SetCurrentAnimationState(idleStates[index]);
                    _idleTimer = 0;
                }
            }
            else
            {
                _idleTimer = 0;
            }
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