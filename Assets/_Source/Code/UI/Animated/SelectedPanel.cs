using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI.Animated
{
    public class SelectedPanel : SelectReaction
    {
        private Tween _tween;
        [SerializeField] private float duration = 0.35f;
        [SerializeField] private Ease ease = Ease.OutCubic;

        private void Awake()
        {
            transform.localScale = new Vector3(0f, 1f, 1f);
            _tween = transform.DOScaleX(1f, duration)
                .SetAutoKill(false)
                .SetUpdate(true)
                .Pause().SetEase(ease);
        }

        protected override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            
            Show();
        }

        protected override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            
            Hide();
        }

        private void Show()
        {
            _tween.PlayForward();
        }

        private void Hide()
        {
            _tween.PlayBackwards();
        }
    }
}