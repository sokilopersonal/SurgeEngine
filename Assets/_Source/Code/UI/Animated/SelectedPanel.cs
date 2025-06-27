using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI.Animated
{
    public class SelectedPanel : SelectReaction
    {
        private Tween _tween;

        private void Awake()
        {
            transform.localScale = new Vector3(0f, 1f, 1f);
            _tween = transform.DOScaleX(1f, 0.2f)
                .SetAutoKill(false)
                .SetUpdate(true)
                .Pause().SetEase(Ease.OutCubic);
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