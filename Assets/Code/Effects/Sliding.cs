using UnityEngine;
using DG.Tweening;

namespace SurgeEngine.Code.ActorEffects
{
    public class Sliding : Effect
    {
        [SerializeField] private TrailRenderer slidingTrail;
        Tweener colorTween;
        private void Start()
        {
            slidingTrail.emitting = true;
            slidingTrail.startColor = Color.clear;
            slidingTrail.endColor = Color.clear;
        }
        public override void Toggle(bool value)
        {
            base.Toggle(value);
            colorTween.Kill(true);
            Color targetColor = value ? Color.white : Color.clear;
            colorTween = DOVirtual.Color(slidingTrail.startColor, targetColor, 0.25f, (Color color) =>
            {
                slidingTrail.startColor = color;
                slidingTrail.endColor = color;
            });
        }
    }
}