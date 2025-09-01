using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Effects
{
    public class StompEffect : Effect
    {
        [SerializeField] private ParticleSystem stompLandParticle;

        public void Land()
        {
            stompLandParticle.Play(true);
        }
    }
}