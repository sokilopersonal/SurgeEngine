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
        
        public static Vector3 GetCameraMatrixPosition(Camera camera, float x, float y)
        {
            var screenPosition = new Vector3(x, y, camera.nearClipPlane);
            
            Matrix4x4 projectionMatrix = camera.projectionMatrix;
            Matrix4x4 viewMatrix = camera.worldToCameraMatrix;
            Matrix4x4 viewProjectionMatrix = projectionMatrix * viewMatrix;

            Vector3 ndcPosition = new Vector3(
                screenPosition.x / Screen.width * 2 - 1,
                screenPosition.y / Screen.height * 2 - 1,
                screenPosition.z
            );
            
            Matrix4x4 inverseViewProjection = viewProjectionMatrix.inverse;
            Vector3 worldPosition = inverseViewProjection.MultiplyPoint(ndcPosition);

            return worldPosition;
        }
    }
}