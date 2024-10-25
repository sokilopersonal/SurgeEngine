using System;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorStats : ActorComponent
    {
        public MoveParameters moveParameters;
        public JumpParameters jumpParameters;
        public HomingParameters homingParameters;
        public StompParameters stompParameters;
        public float turnRate;
        public float currentSpeed;
        public float currentVerticalSpeed;
        public Vector3 movementVector;
        public Vector3 planarVelocity;
        public Vector3 inputDir;
        public Vector3 groundNormal;
        public Vector3 transformNormal;
        public float moveDot;
        public bool skidding;
        public float groundAngle;
        public ContactBase lastContactObject;
        public bool isGrounded;
        public bool isInAir;
        [HideInInspector] public float gravity;
        [field: SerializeField] public float startGravity { get; private set; }
        public Transform homingTarget;

        private Rigidbody _rigidbody;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            startGravity = Mathf.Abs(Physics.gravity.y);
            gravity = startGravity;

            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            currentSpeed = _rigidbody.linearVelocity.magnitude;
            currentVerticalSpeed = _rigidbody.linearVelocity.y;
            
            moveDot = Vector3.Dot(inputDir.normalized, _rigidbody.linearVelocity.normalized);
            skidding = moveDot < moveParameters.skidThreshold;

            var state = actor.stateMachine.CurrentState;
            isGrounded = state is FStateGround;
            isInAir = state is FStateAir or FStateAirBoost or FStateJump or FStateSpecialJump;
        }

        public float GetForwardSignedAngle()
        {
            Vector3 f = Vector3.ProjectOnPlane(actor.transform.forward, Vector3.up).normalized;
            Vector3 c = Vector3.ProjectOnPlane(actor.camera.GetCameraTransform().forward, Vector3.up).normalized;
            return Vector3.SignedAngle(f, c, -Vector3.up);
        }

        public float GetUpwardSignedAngle()
        {
            Vector3 f = Vector3.ProjectOnPlane(actor.transform.up, Vector3.up).normalized;
            Vector3 c = Vector3.ProjectOnPlane(actor.camera.GetCameraTransform().up, Vector3.up).normalized;
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