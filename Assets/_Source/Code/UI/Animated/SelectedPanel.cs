using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine._Source.Code.UI.Animated
{
    public class SelectedPanel : SelectReaction
    {
        private Tween _tween;
        private Tween _tintTween;
        [SerializeField] private float duration = 0.35f;
        [SerializeField] private Ease ease = Ease.OutCubic;
        [SerializeField] private Color baseColor = Color.white;
        [SerializeField] private Color lowTintColor = new(0.4f, 0.4f, 0.4f);

        private void Awake()
        {
            transform.localScale = new Vector3(0f, 1f, 1f);
            _tween = transform.DOScaleX(1f, duration)
                .SetAutoKill(false)
                .SetUpdate(true)
                .Pause().SetEase(ease);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            
            Show();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            
            Hide();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);

            LowTint();
        }

        public override void OnClick(BaseEventData eventData)
        {
            base.OnClick(eventData);
            
            LowTint();
        }

        private void LowTint()
        {
            var image = GetComponent<Image>();
            _tintTween = image.DOColor(baseColor, 0.25f).From(lowTintColor).SetUpdate(true);
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