using SurgeEngine.Code.ActorStates.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorStats : ActorComponent
    {
        public float turnRate;
        public Vector3 movementVector;
        public Vector3 planarVelocity;
        public Vector3 inputDir;
        public Vector3 groundNormal;
        public Vector3 transformNormal;

        public FBoost boost;
        
        public bool boosting;

        private void Start()
        {
            boost = actor.stateMachine.GetSubState<FBoost>();
        }

        private void Update()
        {
            boosting = actor.input.BoostHeld;
            boost.active = boosting;
        }

        public float GetForwardSignedAngle()
        {
            return Vector3.SignedAngle(actor.transform.forward, actor.camera.GetCameraTransform().forward, -groundNormal);
        }
        
        public float GetUpwardSignedAngle()
        {
            return Vector3.SignedAngle(actor.transform.up, Vector3.up, Vector3.down);
        }
    }
}