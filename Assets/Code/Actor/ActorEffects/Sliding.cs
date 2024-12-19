using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class Sliding : Effect
    {
        [SerializeField] private TrailRenderer slidingTrail;

        public override void Toggle(bool value)
        {
            base.Toggle(value);
            slidingTrail.emitting = value;
        }
    }
}