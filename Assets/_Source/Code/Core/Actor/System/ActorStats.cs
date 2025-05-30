using System;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorStats : ActorComponent
    {
        public StompParameters stompParameters;
        public float turnRate;
        public float currentSpeed;
        public float currentVerticalSpeed;
        public Vector3 movementVector;
        public Vector3 planarVelocity;
        public Vector3 inputDir;
        public Vector3 groundNormal;
        public float moveDot;
        public bool skidding;
        public float groundAngle;
        public ContactBase lastContactObject;
        public bool isGrounded;
        public bool isInAir;
        [HideInInspector] public float gravity;
        [field: SerializeField] public float startGravity { get; private set; }
        public HomingTarget homingTarget;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            startGravity = Mathf.Abs(Physics.gravity.y);
            gravity = startGravity;

            _rigidbody = GetComponent<Rigidbody>();
        }
        
        public void OnInit() {}

        private void Update()
        {
            currentSpeed = _rigidbody.linearVelocity.magnitude;
            currentVerticalSpeed = _rigidbody.linearVelocity.y;
            
            moveDot = Vector3.Dot(Actor.Kinematics.GetInputDir().normalized, _rigidbody.linearVelocity.normalized);

            FState state = Actor.stateMachine.CurrentState;
            isGrounded = state is FStateGround;
            isInAir = state is FStateAir or FStateAirBoost or FStateJump or FStateSpecialJump;
            
            if (isInAir)
            {
                homingTarget = SonicTools.FindHomingTarget();
            }
            else
            {
                homingTarget = null;
            }
        }

        public float GetForwardSignedAngle()
        {
            Vector3 forward = Actor.transform.forward;
            Vector3 f = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
            Vector3 c = Vector3.ProjectOnPlane(Actor.Camera.GetCameraTransform().forward, Vector3.up).normalized;
            float dot = Vector3.Dot(Actor.transform.up, Vector3.down);
            return f.SignedAngleByAxis(c, dot > 0 ? Vector3.up : Vector3.down);
        }

        public float GetUpwardSignedAngle()
        {
            Vector3 f = Vector3.ProjectOnPlane(Actor.transform.up, Vector3.up).normalized;
            Vector3 c = Vector3.ProjectOnPlane(Actor.Camera.GetCameraTransform().up, Vector3.up).normalized;
            return Vector3.SignedAngle(f, c, -Vector3.up);
        }

    }

    [Serializable]
    public class StompParameters
    {
        public float stompSpeed;
        public AnimationCurve stompCurve;
    }
}