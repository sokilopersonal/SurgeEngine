using System.Collections;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class DirectionalThorn : StageObject
    {
        [SerializeField] private float moveTime = 0.5f;
        [SerializeField] private float onTime = 2f;
        [SerializeField] private float offTime = 3f;
        [SerializeField] private int phase;
        [SerializeField] private Transform thorn;

        private const float OnHeight = 0f;
        private const float OffHeight = -1.5f;

        private void Awake()
        {
            if (phase == 0)
            {
                thorn.localPosition = new Vector3(thorn.localPosition.x, OnHeight, thorn.localPosition.z);
            }
            else
            {
                StartCoroutine(ThornCycle());
            }
        }

        private IEnumerator ThornCycle()
        {
            while (true)
            {
                yield return MoveThorn(OffHeight, OnHeight, moveTime);
                yield return new WaitForSeconds(onTime);
                yield return MoveThorn(OnHeight, OffHeight, moveTime);
                yield return new WaitForSeconds(offTime);
            }
        }

        private IEnumerator MoveThorn(float startHeight, float endHeight, float duration)
        {
            float elapsed = 0f;
            Vector3 startPos = thorn.localPosition;
            startPos.y = startHeight;
            Vector3 endPos = startPos;
            endPos.y = endHeight;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                thorn.localPosition = Vector3.Lerp(startPos, endPos, Easings.Get(Easing.InOutSine, elapsed / duration));
                yield return null;
            }

            thorn.localPosition = endPos;
        }
    }

}