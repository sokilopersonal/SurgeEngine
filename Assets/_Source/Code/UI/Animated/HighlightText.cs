using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI.Animated
{
    public class HighlightText : SelectReaction
    {
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private Color baseColor = Color.white;
        [SerializeField] private float duration = 0.3f;

        private TMP_Text _text;
        private Tween colorTween;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _text.color = baseColor;

            colorTween = _text.DOColor(highlightColor, duration)
                .SetAutoKill(false)
                .SetUpdate(true)
                .Pause();
        }

        protected override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            colorTween.PlayForward();
        }

        protected override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            colorTween.PlayBackwards();
        }
    }
}