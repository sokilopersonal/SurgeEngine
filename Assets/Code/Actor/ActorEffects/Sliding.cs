using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class Sliding : Effect
    {
        [SerializeField] private TrailRenderer slidingTrail;
        bool toggled = false;
        Color targetColor;
        const float colorSpeed = 10f;
        private void Start()
        {
            slidingTrail.emitting = true;
            slidingTrail.startColor = Color.clear;
            slidingTrail.endColor = Color.clear;
        }
        private void LerpTrail()
        {
            targetColor = toggled ? Color.white : Color.clear;
            slidingTrail.startColor = Color.Lerp(slidingTrail.startColor, targetColor, Time.deltaTime * colorSpeed);
            slidingTrail.endColor = Color.Lerp(slidingTrail.endColor, targetColor, Time.deltaTime * colorSpeed);
        }
        public override void Toggle(bool value)
        {
            base.Toggle(value);
            toggled = value;
        }
        private void Update()
        {
            LerpTrail();
        }
    }
}