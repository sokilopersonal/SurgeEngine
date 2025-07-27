using Unity.Mathematics;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Custom
{
    public static class SurgeMath
    {
        public static Vector3 AxisSlerp(Vector3 from, Vector3 to, float t, Vector3 axis)
        {
            axis.Normalize();
            Vector3 projFrom = Vector3.ProjectOnPlane(from, axis).normalized;
            Vector3 projTo   = Vector3.ProjectOnPlane(to,   axis).normalized;
            float angle      = Vector3.SignedAngle(projFrom, projTo, axis) * t;
            Quaternion rot   = Quaternion.AngleAxis(angle, axis);
            float len        = Mathf.Lerp(from.magnitude, to.magnitude, t);
            return rot * projFrom * len;
        }
        
        public static float SignedAngleByAxis(this Vector3 v1, Vector3 v2, Vector3 axis) 
        {
            Vector3 right = Vector3.Cross(v2, axis);
            v2 = Vector3.Cross(axis, right);
            return Mathf.Atan2(Vector3.Dot(v1, right), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }
    }
}