using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SurgeEngine.Code.UI.Menus;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class Pause : Page
    {
        [SerializeField] private RectTransform parent, firstBox, secondBox, grid;

        private float _startWidth;
        private float _endY;
        private float _endHeight;
        private float _duration;

        private void Start()
        {
            _startWidth = 500f;
            _endY = 175f;
            _endHeight = 660f;
            _duration = 0.525f;
        }

        protected override void InsertIntroAnimations()
        {
            AnimationSequence.Append(Group.DOFade(1f, 0.2f).From(0));
            AnimationSequence.Join(grid.DOSizeDelta(new Vector2(grid.sizeDelta.x, 2500f), _duration * 1.2f).SetEase(Ease.OutCubic).From(new Vector2(grid.sizeDelta.x, 0)));
            AnimationSequence.Join(parent.DOAnchorPosY(_endY, _duration).SetEase(Ease.OutCubic).From(new Vector2(0, 0)));
            AnimationSequence.Join(secondBox.DOSizeDelta(new Vector2(_startWidth, _endHeight), _duration).SetEase(Ease.OutCubic).From(new Vector2(_startWidth, 10)));
            AnimationSequence.Join(firstBox.DOSizeDelta(new Vector2(_startWidth, _endHeight), _duration).SetEase(Ease.OutCubic).SetDelay(0.1f).From(new Vector2(_startWidth, 10)));
        }

        protected override void InsertOutroAnimations()
        {
            AnimationSequence.Append(Group.DOFade(0f, 0.2f).From(1));
            AnimationSequence.Join(parent.DOAnchorPosY(0, _duration).SetEase(Ease.OutCubic).From(new Vector2(0, _endY)));
            AnimationSequence.Join(secondBox.DOSizeDelta(new Vector2(_startWidth, 10), _duration).SetEase(Ease.OutCubic).From(new Vector2(_startWidth, _endHeight)));
            AnimationSequence.Join(firstBox.DOSizeDelta(new Vector2(_startWidth, 10), _duration).SetEase(Ease.OutCubic).SetDelay(0.1f).From(new Vector2(_startWidth, _endHeight)));
        }
    }
}