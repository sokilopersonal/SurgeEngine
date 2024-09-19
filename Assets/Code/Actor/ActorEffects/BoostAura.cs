using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class BoostAura : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems;

        public float airBoostDuration;
        
        private void OnEnable()
        {
            foreach (var gme in particleSystems)
            {
                gme.Play();
            }
        }
        
        private void OnDisable()
        {
            foreach (var gme in particleSystems)
            {
                gme.Stop();
            }
        }
    }
}