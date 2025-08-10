using DG.Tweening;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Effects
{
    public class ParaloopEffect : Effect
    {
        [SerializeField] private TrailRenderer trail1, trail2;
        [HideInInspector] public CharacterBase sonicContext;
        [HideInInspector] public Vector3 startPoint;
        
        private bool toggled;
        Tweener colorTween;
        
        public void Start()
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
            if (toggled)
                return;

            toggled = value;

            colorTween.Kill(true);
            Color targetColor = value ? Color.white : Color.clear;
            colorTween = DOVirtual.Color(trail1.startColor, targetColor, 0.5f, (Color color) =>
            {
                trail1.startColor = color;
                trail1.endColor = color;
                trail2.startColor = color;
                trail2.endColor = color;
            });
        }

        private void Update()
        {  
            if (!toggled || sonicContext == null)
                return;

            if (sonicContext.Kinematics.Speed < sonicContext.Config.minParaloopSpeed || Vector3.Distance(sonicContext.Kinematics.Rigidbody.position, startPoint) > 50f)
            {
                toggled = false;
                Toggle(false);
            }
        }
    }
}