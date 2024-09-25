using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorStats : ActorComponent
    {
        public MoveParameters moveParameters;
        public JumpParameters jumpParameters;
        public HomingParameters homingParameters;
        public float turnRate;
        public float currentSpeed;
        public float currentVerticalSpeed;
        public Vector3 movementVector;
        public Vector3 planarVelocity;
        public Vector3 inputDir;
        public Vector3 groundNormal;
        public Vector3 transformNormal;
        public bool skidding;
        public float groundAngle;
        public ContactBase lastContactObject;
        public bool isGrounded;
        public bool isInAir;

        private Rigidbody _rigidbody;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            currentSpeed = _rigidbody.linearVelocity.magnitude;
            currentVerticalSpeed = _rigidbody.linearVelocity.y;
            
            float skiddingDot = Vector3.Dot(inputDir.normalized, planarVelocity.normalized);
            skidding = skiddingDot < moveParameters.skidThreshold;

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
}