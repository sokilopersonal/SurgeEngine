using System;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    public class AutoScroll : MonoBehaviour
    {
        [SerializeField] private bool isSmoothing = true;
        [SerializeField] private float scrollSpeed = 8f;
        
        private ScrollRect _scrollRect;
        private RectTransform _rect;
        private Vector2 _targetPosition;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _targetPosition = _scrollRect.content.localPosition;
        }

        private void Update()
        {
            if (_rect)
            {
                if (isSmoothing)
                {
                    _scrollRect.content.localPosition = Vector2.Lerp(_scrollRect.content.localPosition, _targetPosition, 
                        Time.unscaledDeltaTime * scrollSpeed);
                }
                else
                {
                    _scrollRect.content.localPosition = _targetPosition;
                }
            }
        }

        public void ScrollTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();
            _rect = target;
            
            if (_rect)
            {
                Vector2 viewLocalPosition = _scrollRect.viewport.localPosition;
                Vector2 targetLocalPosition = target.localPosition;
            
                Vector2 newTargetLocalPosition = new Vector2(
                    _scrollRect.content.localPosition.x, 
                    0-(viewLocalPosition.y+targetLocalPosition.y));
                
                _targetPosition = newTargetLocalPosition;
            }
        }
    }
}