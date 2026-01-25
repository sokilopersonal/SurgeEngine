using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Custom
{
    public static class SurgeMath
    {
        public static Vector3 GetMovementDirectionProjectedOnPlane(Vector3 movement, Vector3 groundNormal, Vector3 upDirection)
        {
            Vector3 movementProjectedOnPlane = Vector3.ProjectOnPlane(movement, groundNormal);
            Vector3 axisToRotateAround = Vector3.Cross(movement, upDirection);
            float angle = Vector3.SignedAngle(movement, movementProjectedOnPlane, axisToRotateAround);
            Quaternion rotation = Quaternion.AngleAxis(angle, axisToRotateAround);
            return (rotation * movement).normalized;
        }
        
        public static float SignedAngleByAxis(this Vector3 v1, Vector3 v2, Vector3 axis) 
        {
            Vector3 right = Vector3.Cross(v2, axis);
            v2 = Vector3.Cross(axis, right);
            return Mathf.Atan2(Vector3.Dot(v1, right), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        public static float Damp(float smoothing, float source)
        {
            return 1 - Mathf.Pow(smoothing, source);
        }
        
        public static void SplitPlanarVector(Vector3 vector, Vector3 normal, out Vector3 planar, out Vector3 vertical)
        {
            planar = Vector3.ProjectOnPlane(vector, normal);
            vertical = vector - Vector3.ProjectOnPlane(vector, normal);
        }
    }
}