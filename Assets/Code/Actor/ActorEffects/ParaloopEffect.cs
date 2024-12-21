using UnityEngine;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSoundEffects;
using DG.Tweening;

namespace SurgeEngine.Code.ActorEffects
{
    public class ParaloopEffect : Effect
    {
        [SerializeField] private TrailRenderer trail1, trail2;
        [HideInInspector] public Actor sonicContext = null;
        [HideInInspector] public Vector3 startPoint;
        bool toggled = false;
        ParaloopSound sound;
        Color targetColor;
        const float colorSpeed = 10f;
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

            if (value)
            {
                toggled = true;
                if (!sound)
                    sound = sonicContext.sounds.GetComponent<ParaloopSound>();
                sound.Play();
            }
        }

        private void LerpTrails()
        {
            targetColor = toggled ? Color.white : Color.clear;

            trail1.startColor = Color.Lerp(trail1.startColor, targetColor, Time.deltaTime * colorSpeed);
            trail1.endColor = Color.Lerp(trail1.endColor, targetColor, Time.deltaTime * colorSpeed);

            trail2.startColor = Color.Lerp(trail2.startColor, targetColor, Time.deltaTime * colorSpeed);
            trail2.endColor = Color.Lerp(trail2.endColor, targetColor, Time.deltaTime * colorSpeed);
        }

        private void Update()
        {
            LerpTrails();
            
            if (!toggled || sonicContext == null)
                return;

            if (sonicContext.kinematics.HorizontalSpeed < sonicContext.config.minParaloopSpeed || Vector3.Distance(sonicContext.kinematics.Rigidbody.position, startPoint) > 50f)
            {
                toggled = false;
                Toggle(false);
            }
        }
    }
}