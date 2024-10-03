using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class PlayParticlesOnContact : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particle;

        private ContactBase contact;
        
        private void Awake()
        {
            contact = GetComponent<ContactBase>();
        }

        private void OnEnable()
        {
            contact.OnContact += OnContact;
        }
        
        private void OnDisable()
        {
            contact.OnContact -= OnContact;
        }

        private void OnContact(ContactBase obj)
        {
            particle.Stop(true);
            particle.Play(true);
        }
    }
}