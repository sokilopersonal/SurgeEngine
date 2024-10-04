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

        public static Vector3 GetPositionInCameraProjection(Camera camera, Vector2 pos)
        {
            var m = camera.projectionMatrix * camera.worldToCameraMatrix;
            var p = Vector3.zero;
            p.x = (pos.x * 2 - 1) / m.m00;
            p.y = (pos.y * 2 - 1) / m.m11;
            p.z = 1 / m.m33;
            return m.MultiplyPoint3x4(p);
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
        
        public static Vector3 DivideVector3(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x,
                a.y / b.y,
                a.z / b.z);
        }
    }
}