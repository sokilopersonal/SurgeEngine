using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class Sliding : MonoBehaviour
    {
        [SerializeField] private ParticleSystem slidingParticle;

        private void OnEnable()
        {
            slidingParticle.Play(true);
        }

        private void OnDisable()
        {
            slidingParticle.Stop(true);
        }
    }
}