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
        
        public float GetSignedAngle()
        {
            return Vector3.SignedAngle(planarVelocity, inputDir, groundNormal);
        }
    }
}