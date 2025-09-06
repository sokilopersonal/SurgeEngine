using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Player
{
    public class PlayParticlesOnContact : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particle;

        private StageObject contact;
        
        private void Awake()
        {
            contact = GetComponent<StageObject>();
        }

        private void OnEnable()
        {
            contact.OnContact += OnContact;
        }
        
        private void OnDisable()
        {
            contact.OnContact -= OnContact;
        }

        private void OnContact(StageObject obj)
        {
            particle.Stop(true);
            particle.Play(true);
        }
    }
}