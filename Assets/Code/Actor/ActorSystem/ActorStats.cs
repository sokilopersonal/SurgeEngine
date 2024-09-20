using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorStats : ActorComponent
    {
        public MoveParameters moveParameters;
        public JumpParameters jumpParameters;
        public float turnRate;
        public float currentSpeed;
        public float currentVerticalSpeed;
        public Vector3 movementVector;
        public Vector3 planarVelocity;
        public Vector3 inputDir;
        public Vector3 groundNormal;
        public Vector3 transformNormal;
        public float groundAngle;

        private Rigidbody _rigidbody;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            currentSpeed = planarVelocity.magnitude;
            currentVerticalSpeed = _rigidbody.linearVelocity.y;
        }

        public float GetForwardSignedAngle()
        {
            return Vector3.SignedAngle(Vector3.Cross(actor.transform.right, Vector3.up), Vector3.Cross(actor.camera.GetCameraTransform().right, Vector3.up), -Vector3.up);
        }

        public float GetUpwardSignedAngle()
        {
            return Vector3.SignedAngle(actor.transform.up, Vector3.up, Vector3.down);
        }
    }
}