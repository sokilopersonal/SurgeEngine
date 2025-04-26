using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class PointMarkerLoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;

        private Tween _tween;

        private void Awake()
        {
            
        }

        public IEnumerator Play()
        {
            yield return group.DOFade(1f, 1f).From(0).WaitForCompletion();
            yield return new WaitForSecondsRealtime(1.25f);
        }

        public IEnumerator Hide()
        {
            yield return group.DOFade(0f, 0.25f).WaitForCompletion();
            Destroy(gameObject);
        }
    }
}