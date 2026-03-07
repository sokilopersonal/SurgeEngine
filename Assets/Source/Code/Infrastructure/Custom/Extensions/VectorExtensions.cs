using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Custom.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ProjectOnUp(this Vector3 v)
        {
            return v - Vector3.Project(v, Vector3.up);
        }
    }
}