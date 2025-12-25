using DG.Tweening;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Effects
{
    public class ParaloopEffect : Effect
    {
        [SerializeField] private TrailRenderer trail1, trail2;

        private bool _toggled;
        private Tween _colorTween;
        
        public void Awake()
        {
            trail1.emitting = true;
            trail1.startColor = Color.clear;
            trail1.endColor = Color.clear;

            trail2.emitting = true;
            trail2.startColor = Color.clear;
            trail2.endColor = Color.clear;
        }
        
        public override void Toggle(bool value)
        {
            _toggled = value;

            _colorTween?.Kill(true);
            Color targetColor = value ? Color.white : Color.clear;
            _colorTween = DOVirtual.Color(trail1.startColor, targetColor, 0.5f, color =>
            {
                trail1.startColor = color;
                trail1.endColor = color;
                trail2.startColor = color;
                trail2.endColor = color;
            });
        }
    }
}