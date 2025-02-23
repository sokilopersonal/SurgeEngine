using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class Pause : Menu
    {
        [SerializeField] private RectTransform parent, firstBox, secondBox, grid;

        private Sequence _sequence;
        
        private float _startWidth;
        private float _endY;
        private float _endHeight;
        private float _duration;

        private void Start()
        {
            _startWidth = 500f;
            _endY = 175f;
            _endHeight = 650f;
            _duration = 0.525f;
        }

        public override async Task Open()
        {
            base.Open();
            
            _sequence?.Kill(true);
            _sequence = DOTween.Sequence();
            _sequence.Append(Group.DOFade(1f, 0.2f).From(0));
            _sequence.Join(grid.DOSizeDelta(new Vector2(grid.sizeDelta.x, 2500f), _duration * 1.2f).SetEase(Ease.OutCubic).From(new Vector2(grid.sizeDelta.x, 0)));
            _sequence.Join(parent.DOAnchorPosY(_endY, _duration).SetEase(Ease.OutCubic).From(new Vector2(0, 0)));
            _sequence.Join(secondBox.DOSizeDelta(new Vector2(_startWidth, _endHeight), _duration).SetEase(Ease.OutCubic).From(new Vector2(_startWidth, 10)));
            _sequence.Join(firstBox.DOSizeDelta(new Vector2(_startWidth, _endHeight), _duration).SetEase(Ease.OutCubic).SetDelay(0.1f).From(new Vector2(_startWidth, 10)));
            _sequence.SetUpdate(true);

            await _sequence.AsyncWaitForCompletion();
        }

        public override async Task Close()
        {
            _sequence?.Kill(true);
            _sequence = DOTween.Sequence();
            _sequence.Append(Group.DOFade(0f, 0.2f).From(1));
            _sequence.Join(parent.DOAnchorPosY(0, _duration).SetEase(Ease.InCubic).From(new Vector2(0, _endY)));
            _sequence.Join(secondBox.DOSizeDelta(new Vector2(_startWidth, 10), _duration).SetEase(Ease.InCubic).From(new Vector2(_startWidth, _endHeight)));
            _sequence.Join(firstBox.DOSizeDelta(new Vector2(_startWidth, 10), _duration).SetEase(Ease.InCubic).SetDelay(0.1f).From(new Vector2(_startWidth, _endHeight)));
            _sequence.SetUpdate(true);
            
            await _sequence.AsyncWaitForCompletion();
        }
    }
}