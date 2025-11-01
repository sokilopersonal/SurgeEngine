using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Effects
{
    public class SweepEffect : Effect
    {
        [SerializeField] private Light pointLight;
        private float startIntensity;

        private void Start()
        {
            startIntensity = pointLight.intensity;
        }

        public override void Toggle(bool value)
        {
            if (value)
            {
                pointLight.intensity = 0;
                pointLight.enabled = true;
                pointLight.DOKill();
                pointLight.DOIntensity(startIntensity, 0.5f).SetDelay(0.1f).OnComplete(() =>
                {
                    pointLight.DOIntensity(0f, 0.5f).OnComplete(() =>
                    {
                        pointLight.enabled = false;
                    });
                });
            }
            else
            {
                pointLight.DOIntensity(0, 0.5f).OnComplete(() =>
                {
                    pointLight.enabled = false;
                });
            }
            base.Toggle(value);
        }

        public override void Clear()
        {
            pointLight.enabled = false;
            pointLight.intensity = 0f;
            base.Clear();
        }
    }
}