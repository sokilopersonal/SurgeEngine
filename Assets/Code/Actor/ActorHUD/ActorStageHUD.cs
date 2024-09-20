using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using UnityEngine;

namespace SurgeEngine.Code.ActorHUD
{
    public class ActorStageHUD : MonoBehaviour
    {
        [SerializeField] private RingHUD ringHUDPrefab; // Префаб RingHUD
        [SerializeField] private Camera mainCamera; // Камера, к углу которой нужно переместить объект
        
        private Actor _actor => ActorContext.Context;

        private void OnEnable()
        {
            ActorEvents.OnRingCollected += OnRingCollected;
        }

        private void OnDisable()
        {
            ActorEvents.OnRingCollected -= OnRingCollected;
        }

        private void OnRingCollected(Ring obj)
        {
            // Создание экземпляра RingHUD на позиции кольца
            RingHUD ringHUDInstance = Instantiate(ringHUDPrefab, obj.transform.position, Quaternion.identity);

            // Вычисление целевой позиции в углу камеры
            Vector3 targetWorldPosition = GetCameraCornerWorldPosition(mainCamera, CameraCorner.TopRight);

            // Инициализация движения кольца к позиции в мире
            ringHUDInstance.Initialize(targetWorldPosition, 1.0f); // Передаем целевую позицию в мире и время анимации
        }

        // Метод для получения позиции угла камеры в мировых координатах
        private Vector3 GetCameraCornerWorldPosition(Camera camera, CameraCorner corner)
        {
            // Определяем экранные координаты для выбранного угла
            Vector3 screenPosition = Vector3.zero;
            switch (corner)
            {
                case CameraCorner.TopLeft:
                    screenPosition = new Vector3(0, Screen.height, camera.nearClipPlane);
                    break;
                case CameraCorner.TopRight:
                    screenPosition = new Vector3(Screen.width, Screen.height, camera.nearClipPlane);
                    break;
                case CameraCorner.BottomLeft:
                    screenPosition = new Vector3(0, 0, camera.nearClipPlane);
                    break;
                case CameraCorner.BottomRight:
                    screenPosition = new Vector3(Screen.width, 0, camera.nearClipPlane);
                    break;
            }

            // Преобразуем экранные координаты в мировые, используя матрицы
            Matrix4x4 projectionMatrix = camera.projectionMatrix;
            Matrix4x4 viewMatrix = camera.worldToCameraMatrix;
            Matrix4x4 viewProjectionMatrix = projectionMatrix * viewMatrix;

            Vector3 ndcPosition = new Vector3(
                (screenPosition.x / Screen.width) * 2 - 1,
                (screenPosition.y / Screen.height) * 2 - 1,
                screenPosition.z
            );

            // Инвертируем матрицу для получения позиции в мировых координатах
            Matrix4x4 inverseViewProjection = viewProjectionMatrix.inverse;
            Vector3 worldPosition = inverseViewProjection.MultiplyPoint(ndcPosition);

            return worldPosition;
        }

        // Перечисление для выбора угла камеры
        private enum CameraCorner
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }
    }
}