using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class Stomping : MonoBehaviour
    {
        [SerializeField] private ParticleSystem stompParticle;
        [SerializeField] private ParticleSystem stompLandParticle;

        private void OnEnable()
        {
            stompParticle.Play(true);
        }
        
        private void OnDisable()
        {
            stompParticle.Stop(true);
        }

        public void Land()
        {
            stompLandParticle.Play(true);
        }
    }
}