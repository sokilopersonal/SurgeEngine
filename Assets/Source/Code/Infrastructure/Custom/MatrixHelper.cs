using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Custom
{
    public static class MatrixHelper
    {
        public static Vector3 GetMatrixRectTransformPosition(RectTransform rectTransform, Camera camera, float distance)
        {
            Vector3 rect = rectTransform.position;
            Vector3 screenPos = rect;
            screenPos.z = distance;

            Camera cam = camera;
            float ndcX = screenPos.x / cam.pixelWidth * 2 - 1;
            float ndcY = screenPos.y / cam.pixelHeight * 2 - 1;

            Vector4 ndcNear = new Vector4(ndcX, ndcY, -1f, 1f);
            Matrix4x4 invProjectionMatrix = cam.projectionMatrix.inverse;
            Matrix4x4 cameraToWorldMatrix = cam.cameraToWorldMatrix;

            Vector4 worldNearH = cameraToWorldMatrix * invProjectionMatrix * ndcNear;
            Vector3 worldNear = worldNearH / worldNearH.w;

            Vector3 dir = (worldNear - cam.transform.position).normalized;
            Vector3 worldPos = cam.transform.position + dir * distance;
            
            return worldPos;
        }
    }
}