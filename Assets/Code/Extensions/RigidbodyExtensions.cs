using UnityEngine;

namespace SurgeEngine.Code.Custom
{
    public static class RigidbodyExtensions
    {
        public static Vector3 GetHorizontalVelocity(this Rigidbody rb)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            return vel;
        }
        
        public static float GetHorizontalMagnitude(this Rigidbody rb)
        {
            Vector3 vel = GetHorizontalVelocity(rb);
            return vel.magnitude;
        }
    }
}