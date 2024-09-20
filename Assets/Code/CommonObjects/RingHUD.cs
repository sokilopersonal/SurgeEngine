using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class RingHUD : MonoBehaviour
    {
        public void Initialize(Vector3 targetWorldPosition, float time)
        {
            StartCoroutine(AnimateRing(targetWorldPosition, time));
        }

        private System.Collections.IEnumerator AnimateRing(Vector3 targetWorldPosition, float time)
        {
            float elapsed = 0;
            Vector3 startPosition = transform.position;

            // Анимация перемещения в мир
            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, targetWorldPosition, elapsed / time);
                yield return null;
            }

            // Убедимся, что кольцо в конечной позиции
            transform.position = targetWorldPosition;
        }
    }
}