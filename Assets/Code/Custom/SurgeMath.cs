using Unity.Mathematics;
using UnityEngine;

namespace SurgeEngine.Code.Custom
{
    public static class SurgeMath
    {
        public static void SplitPlanarVector(Vector3 Vector, Vector3 Normal, out Vector3 Planar, out Vector3 Vertical)
        {
            Planar = Vector3.ProjectOnPlane(Vector, Normal);
            Vertical = Vector - Planar;
        }
        
        public static float Smooth(float t, float f = 0.5f, float a1 = 0.1f)
        {
            return Mathf.Approximately(t, 1)
                ? 1
                : Mathf.Clamp01(((1 / (1 - t)) - 1) * Mathf.Pow(1 - Mathf.Pow(1 - f, Time.deltaTime), a1));
        }
        
        public static float3 Vector3ToFloat3(Vector3 vector3)
        {
            return new float3(vector3.x, vector3.y, vector3.z);
        }
        
        public static Vector3 Float3ToVector3(float3 float3)
        {
            return new Vector3(float3.x, float3.y, float3.z);
        }
        
        public static float SignedAngleByAxis(this Vector3 v1, Vector3 v2, Vector3 axis) 
        {
            Vector3 right = Vector3.Cross(v2, axis);
            Vector3.Cross(axis, right);
            return Mathf.Atan2(Vector3.Dot(v1, right), 1) * Mathf.Rad2Deg;
        }

        public static Vector3 Lerp3(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return t < 0 ? Vector3.LerpUnclamped(a, b, t + 1f) : Vector3.LerpUnclamped(b, c, t);
        }
        
        public static float Lerp3(float a, float b, float c, float t)
        {
            return t < 0 ? Mathf.LerpUnclamped(a, b, t + 1f) : Mathf.LerpUnclamped(b, c, t);
        }
    }
}