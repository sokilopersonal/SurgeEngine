using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI.Menus
{
    public class OptionsPage : Page
    {
        [SerializeField] private RectTransform parent, firstBox, secondBox;

        private float _startWidth;
        private float _endY;
        private float _endHeight;

        private void Start()
        {
            _startWidth = 550f;
            _endY = 125f;
            _endHeight = 750f;
        }

        protected override void InsertIntroAnimations()
        {
            AnimationSequence.Join(Group.DOFade(1f, 0.2f).From(0));
            AnimationSequence.Join(parent.DOAnchorPosY(_endY, duration).SetEase(Ease.OutCubic).From(new Vector2(0, 0)));
            AnimationSequence.Join(secondBox.DOSizeDelta(new Vector2(_startWidth, _endHeight), duration).SetEase(Ease.OutCubic).From(new Vector2(_startWidth, 10)));
            AnimationSequence.Join(firstBox.DOSizeDelta(new Vector2(_startWidth, _endHeight), duration).SetEase(Ease.OutCubic).SetDelay(0.1f).From(new Vector2(_startWidth, 10)));
        }

        protected override void InsertOutroAnimations()
        {
            AnimationSequence.Join(Group.DOFade(0f, 0.2f).From(1));
            AnimationSequence.Join(parent.DOAnchorPosY(0, duration).SetEase(Ease.OutCubic).From(new Vector2(0, _endY)));
            AnimationSequence.Join(secondBox.DOSizeDelta(new Vector2(_startWidth, 10), duration).SetEase(Ease.OutCubic).From(new Vector2(_startWidth, _endHeight)));
            AnimationSequence.Join(firstBox.DOSizeDelta(new Vector2(_startWidth, 10), duration).SetEase(Ease.OutCubic).SetDelay(0.1f).From(new Vector2(_startWidth, _endHeight)));
        }
    }
}