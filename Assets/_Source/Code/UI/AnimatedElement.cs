using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class AnimatedElement : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float delay;
        [SerializeField] private Ease easing = Ease.OutQuad;
        [SerializeField] private bool unscaled = true;
        
        public RectTransform RectTransform => rectTransform;
        public float Duration => duration;
        public float Delay => delay;
        public Ease Easing => easing;
        public bool Unscaled => unscaled;
        
        [SerializeReference] public List<ElementAnimation> animations = new();

        private void Awake()
        {
            Setup();
        }

        public void Open()
        {
            foreach (var element in animations)
            {
                element.Open();
            }
        }
        
        public void Close()
        {
            foreach (var element in animations)
            {
                element.Close();
            }
        }
        
        private void OnValidate()
        {
            foreach (var element in animations)
            {
                element?.SetData(this);
            }
        }

        private void Setup()
        {
            if (TryGetComponent(out RectTransform rect))
            {
                rectTransform = rect;
            }
        }
    }

    [Serializable]
    public abstract class ElementAnimation
    {
        protected AnimatedElement data;
        protected Tween tween;
        
        public virtual void SetData(AnimatedElement animatedElement)
        {
            data = animatedElement;
        }
        
        protected Tween ConfigureTween(Tween newTween)
        {
            if (newTween == null) return null;
            
            return newTween
                .SetEase(data.Easing)
                .SetDelay(data.Delay)
                .SetUpdate(data.Unscaled);
        }
        
        protected void KillCurrentTween()
        {
            tween?.Kill(true);
        }
        
        public Tween Open()
        {
            KillCurrentTween();
            tween = ConfigureTween(CreateOpenTween());
            return tween;
        }
        
        public Tween Close()
        {
            KillCurrentTween();
            tween = ConfigureTween(CreateCloseTween());
            return tween;
        }
        
        protected abstract Tween CreateOpenTween();
        protected abstract Tween CreateCloseTween();
        public abstract string GetDisplayName();
    }

    [Serializable]
    public class ElementPositionAnimation : ElementAnimation
    {
        public Vector2 startAnchoredPosition;
        public Vector2 endAnchoredPosition;
        
        public override string GetDisplayName() => "Position Animation";
        
        protected override Tween CreateOpenTween()
        {
            return data.RectTransform.DOAnchorPos(endAnchoredPosition, data.Duration);
        }
        
        protected override Tween CreateCloseTween()
        {
            return data.RectTransform.DOAnchorPos(startAnchoredPosition, data.Duration);
        }
    }

    [Serializable]
    public class ElementScaleAnimation : ElementAnimation
    {
        public Vector3 startScale = Vector3.one;
        public Vector3 endScale = Vector3.zero;
        
        public override string GetDisplayName() => "Scale Animation";
        
        protected override Tween CreateOpenTween()
        {
            return data.RectTransform.DOScale(endScale, data.Duration);
        }
        
        protected override Tween CreateCloseTween()
        {
            return data.RectTransform.DOScale(startScale, data.Duration);
        }
    }

    [Serializable]
    public class ElementRotationAnimation : ElementAnimation
    {
        public Vector3 startRotation = Vector3.zero;
        public Vector3 endRotation = new Vector3(0, 0, 360);
        
        public override string GetDisplayName() => "Rotation Animation";
        
        protected override Tween CreateOpenTween()
        {
            return data.RectTransform.DORotate(endRotation, data.Duration);
        }
        
        protected override Tween CreateCloseTween()
        {
            return data.RectTransform.DORotate(startRotation, data.Duration);
        }
    }

    [Serializable]
    public class ElementFadeAnimation : ElementAnimation
    {
        public float startAlpha = 1f;
        public float endAlpha = 0f;
        private CanvasGroup canvasGroup;
        
        public override string GetDisplayName() => "Fade Animation";
        
        private CanvasGroup GetCanvasGroup()
        {
            if (canvasGroup == null && data != null)
            {
                canvasGroup = data.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = data.gameObject.AddComponent<CanvasGroup>();
                }
            }
            return canvasGroup;
        }
        
        protected override Tween CreateOpenTween()
        {
            var cg = GetCanvasGroup();
            return cg?.DOFade(endAlpha, data.Duration);
        }
        
        protected override Tween CreateCloseTween()
        {
            var cg = GetCanvasGroup();
            return cg?.DOFade(startAlpha, data.Duration);
        }
    }
}