using System;
using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class SelectionBox : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private float scaleDuration = 0.3f;
        [SerializeField] private Ease scaleEase = Ease.OutQuad;

        private Tween _tween;
        private float _startHeight;

        private void Awake()
        {
            Vector2 delta = rect.sizeDelta;
            _startHeight = delta.y;
            delta.y = 0;
            rect.sizeDelta = delta;
        }
        
        public void Select()
        {
            _tween?.Kill(true);
            _tween = rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, _startHeight), scaleDuration).SetEase(scaleEase).From(new Vector2(rect.sizeDelta.x, 0)).SetUpdate(true);
            _tween.SetLink(gameObject);
        }
        
        public void Deselect()
        {
            _tween?.Kill(true);
            _tween = rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, 0), scaleDuration).SetEase(scaleEase).SetUpdate(true);
            _tween.SetLink(gameObject);
        }
    }
}