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
            Character.StateMachine.OnStateEarlyAssign += ChangeAnimationState;
        }

        private void OnDisable()
        {
            Character.StateMachine.OnStateEarlyAssign -= ChangeAnimationState;
        }

        protected virtual void Update()
        {
            var animator = StateAnimator.Animator;
            animator.SetFloat(AnimatorParams.GroundSpeed, Mathf.Clamp(Character.Kinematics.Speed, 0, 30f));
            animator.SetFloat(AnimatorParams.VerticalSpeed, Character.Kinematics.Velocity.y);
            
            float targetSpeedPercent = Mathf.Clamp(Character.Kinematics.Speed / Character.Config.topSpeed, 0.02f, 1.1f);
            float currentSpeedPercent = animator.GetFloat("SpeedPercent");
            animator.SetFloat("SpeedPercent", Mathf.Lerp(currentSpeedPercent, targetSpeedPercent, 10f * Time.deltaTime));

            var characterForward = Vector3.ProjectOnPlane(Character.transform.forward, Vector3.up);
            var camForward = Vector3.ProjectOnPlane(Character.Camera.GetCameraTransform().forward, Vector3.up);
            float camDot  = Vector3.Dot(camForward, characterForward);
            float inputX = Character.Input.MoveVector.x;
            float angle = inputX * camDot;
            
            animator.SetFloat(AnimatorParams.SmoothTurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.SmoothTurnAngle), angle, 4f * Time.deltaTime));
            animator.SetFloat(AnimatorParams.TurnAngle, Mathf.Lerp(animator.GetFloat(AnimatorParams.TurnAngle), angle, 3f * Time.deltaTime));

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