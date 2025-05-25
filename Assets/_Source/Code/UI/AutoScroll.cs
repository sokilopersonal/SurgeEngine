using System;
using System.Collections.Generic;
using SurgeEngine.Code.UI.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    public class AutoScroll : MonoBehaviour
    {
        [SerializeField] private bool isSmoothing = true;
        [SerializeField] private float scrollSpeed = 8f;
        private ScrollRect _scrollRect;
        public ScrollRect ScrollRect => _scrollRect;
        
        private RectTransform _rect;
        private Vector2 _target;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        private void Update()
        {
            if (_rect)
            {
                if (isSmoothing)
                {
                    _scrollRect.normalizedPosition = Vector2.Lerp(_scrollRect.normalizedPosition, _target, scrollSpeed * Time.unscaledDeltaTime);
                }
                else
                {
                    _scrollRect.normalizedPosition = _target;
                }
            }
        }

        public void ScrollTo(RectTransform target, bool quick = false)
        {
            _rect = target;

            if (_rect)
            {
                Canvas.ForceUpdateCanvases();

                var scrollRect = _scrollRect;

                RectTransform content = scrollRect.content;
                RectTransform viewport = scrollRect.viewport;
                Vector2 contentPivot = content.pivot;
                Vector2 contentSize = content.rect.size;
                Vector2 viewportSize = viewport.rect.size;
                Vector2 targetLocalPos = content.InverseTransformPoint(target.position);
                Vector2 targetPos = new Vector2(targetLocalPos.x + contentSize.x * contentPivot.x,
                    targetLocalPos.y + contentSize.y * contentPivot.y);
                Vector2 normalizedPos = new Vector2(0, 1);
                if (scrollRect.vertical && contentSize.y > viewportSize.y)
                {
                    float verticalPos =
                        Mathf.Clamp01((targetPos.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y));
                    normalizedPos.y = verticalPos;
                }

                _target = normalizedPos;
                
                if (quick)
                {
                    _scrollRect.normalizedPosition = _target;
                }
            }
        }
    }
}