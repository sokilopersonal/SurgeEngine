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
        private float _target;

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
                    _scrollRect.verticalNormalizedPosition = Mathf.Lerp(_scrollRect.verticalNormalizedPosition, _target, scrollSpeed * Time.unscaledDeltaTime);
                }
                else
                {
                    _scrollRect.verticalNormalizedPosition = _target;
                }
            }
        }
        
        public void ScrollTo(RectTransform target)
        {
            ScrollTo(target, false);
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
                float normalizedPos = _scrollRect.verticalNormalizedPosition;
                if (scrollRect.vertical && contentSize.y > viewportSize.y)
                {
                    float verticalPos =
                        Mathf.Clamp01((targetPos.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y));
                    normalizedPos = verticalPos;
                }

                _target = normalizedPos;
                
                if (quick)
                {
                    _scrollRect.verticalNormalizedPosition = _target;
                }
            }
        }
    }
}