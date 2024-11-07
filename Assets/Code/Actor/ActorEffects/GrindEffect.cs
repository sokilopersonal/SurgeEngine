using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class GrindEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem grindParticle;        
        
        private void OnEnable()
        {
            grindParticle.Play(true);
        }

        private void OnDisable()
        {
            grindParticle.Stop(true);
        }
    }
}