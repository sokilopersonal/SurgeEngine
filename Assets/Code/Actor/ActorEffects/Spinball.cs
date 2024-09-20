using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class Spinball : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems;

        private void OnEnable()
        {
            foreach (var gte in particleSystems)
            {
                gte.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            foreach (var gte in particleSystems)
            {
                gte.gameObject.SetActive(false);
            }
        }
    }
}