using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Custom.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 Abs(this Vector3 v) => new(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    
        public static bool IsLess(this float value, float other) => value < other;
    }
}