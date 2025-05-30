using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Custom.Extensions
{
    public static class DebugExtensions
    {
        public static void DrawCube(Vector3 center, Vector3 size, Color color = default, float duration = 0)
        {
            if (color == default) color = Color.white;
            Vector3 halfSize = size * 0.5f;
            
            Vector3 p1 = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            Vector3 p2 = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            Vector3 p3 = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            Vector3 p4 = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            
            Vector3 p5 = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            Vector3 p6 = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            Vector3 p7 = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
            Vector3 p8 = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);
            
            // Bottom
            Debug.DrawLine(p1, p2, color, duration);
            Debug.DrawLine(p2, p3, color, duration);
            Debug.DrawLine(p3, p4, color, duration);
            Debug.DrawLine(p4, p1, color, duration);
            
            // Top
            Debug.DrawLine(p5, p6, color, duration);
            Debug.DrawLine(p6, p7, color, duration);
            Debug.DrawLine(p7, p8, color, duration);
            Debug.DrawLine(p8, p5, color, duration);
            
            // Sides
            Debug.DrawLine(p1, p5, color, duration);
            Debug.DrawLine(p2, p6, color, duration);
            Debug.DrawLine(p3, p7, color, duration);
            Debug.DrawLine(p4, p8, color, duration);
        }
    
        public static void DrawCube(Matrix4x4 matrix, Vector3 size, Color color = default, float duration = 0)
        {
            if (color == default) color = Color.white;
            Vector3 halfSize = size * 0.5f;
            
            Vector3 p1 = matrix.MultiplyPoint3x4(new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));
            Vector3 p2 = matrix.MultiplyPoint3x4(new Vector3(halfSize.x, -halfSize.y, -halfSize.z));
            Vector3 p3 = matrix.MultiplyPoint3x4(new Vector3(halfSize.x, -halfSize.y, halfSize.z));
            Vector3 p4 = matrix.MultiplyPoint3x4(new Vector3(-halfSize.x, -halfSize.y, halfSize.z));
            
            Vector3 p5 = matrix.MultiplyPoint3x4(new Vector3(-halfSize.x, halfSize.y, -halfSize.z));
            Vector3 p6 = matrix.MultiplyPoint3x4(new Vector3(halfSize.x, halfSize.y, -halfSize.z));
            Vector3 p7 = matrix.MultiplyPoint3x4(new Vector3(halfSize.x, halfSize.y, halfSize.z));
            Vector3 p8 = matrix.MultiplyPoint3x4(new Vector3(-halfSize.x, halfSize.y, halfSize.z));
            
            // Bottom
            Debug.DrawLine(p1, p2, color, duration);
            Debug.DrawLine(p2, p3, color, duration);
            Debug.DrawLine(p3, p4, color, duration);
            Debug.DrawLine(p4, p1, color, duration);
            
            // Top
            Debug.DrawLine(p5, p6, color, duration);
            Debug.DrawLine(p6, p7, color, duration);
            Debug.DrawLine(p7, p8, color, duration);
            Debug.DrawLine(p8, p5, color, duration);
            
            // Sides
            Debug.DrawLine(p1, p5, color, duration);
            Debug.DrawLine(p2, p6, color, duration);
            Debug.DrawLine(p3, p7, color, duration);
            Debug.DrawLine(p4, p8, color, duration);
        }
    }
}