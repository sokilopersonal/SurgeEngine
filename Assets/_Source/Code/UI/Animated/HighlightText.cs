using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Animated
{
    public class HighlightText : SelectReaction
    {
        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private Color baseColor = Color.white;
        [SerializeField] private float duration = 0.3f;

        private Tween _colorTween;

        private void Awake()
        {
            targetGraphic.color = baseColor;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            
            _colorTween?.Kill(true);
            _colorTween = targetGraphic.DOColor(highlightColor, duration).SetUpdate(true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            
            _colorTween?.Kill(true);
            _colorTween = targetGraphic.DOColor(baseColor, duration).SetUpdate(true);
        }
    }
}