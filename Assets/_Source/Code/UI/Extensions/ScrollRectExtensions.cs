using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Extensions
{
    public static class ScrollRectExtensions
    {
        public static void EnsureVisibility(this ScrollRect scrollRect, RectTransform child, float padding = 0)
        {
            scrollRect.content.anchoredPosition = GetEnsuredVisibilityPosition(scrollRect, child, padding);
        }

        public static Vector2 GetEnsuredVisibilityPosition(this ScrollRect scrollRect, RectTransform child, float padding = 0)
        {
            Debug.Assert(child.parent == scrollRect.content,
                "EnsureVisibility assumes that 'child' is directly nested in the content of 'scrollRect'");

            float viewportHeight = scrollRect.viewport.rect.height;
            Vector2 scrollPosition = scrollRect.content.anchoredPosition;

            float elementTop = child.anchoredPosition.y;
            float elementBottom = elementTop - child.rect.height;

            float visibleContentTop = -scrollPosition.y - padding;
            float visibleContentBottom = -scrollPosition.y - viewportHeight + padding;

            float scrollDelta =
                elementTop > visibleContentTop ? visibleContentTop - elementTop :
                elementBottom < visibleContentBottom ? visibleContentBottom - elementBottom :
                0f;

            scrollPosition.y += scrollDelta;
            
            return scrollPosition;
        }
    }
}