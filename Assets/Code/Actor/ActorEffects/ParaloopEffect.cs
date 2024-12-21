using UnityEngine;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSoundEffects;

namespace SurgeEngine.Code.ActorEffects
{
    public class ParaloopEffect : Effect
    {
        [SerializeField] private TrailRenderer trail1, trail2;
        [HideInInspector] public Actor sonicContext = null;
        [HideInInspector] public Vector3 startPoint;
        bool toggled = false;
        ParaloopSound sound;
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
            
            trail1.emitting = value;
            trail2.emitting = value;
        }

        private void Update()
        {
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