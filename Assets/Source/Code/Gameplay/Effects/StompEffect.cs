using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Effects
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