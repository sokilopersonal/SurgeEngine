using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorStats : ActorComponent
    {
        public float turnRate;
        public float currentSpeed;
        public Vector3 movementVector;
        public Vector3 planarVelocity;
        public Vector3 inputDir;
        public Vector3 groundNormal;
        public Vector3 transformNormal;
        public float groundAngle;

        public FBoost boost;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            boost = actor.stateMachine.GetSubState<FBoost>();
        }

        private void Update()
        {
            currentSpeed = planarVelocity.magnitude;
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