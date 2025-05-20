using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Player
{
    public class PlaySoundOnContact : MonoBehaviour
    {
        [SerializeField] private EventReference soundReference;
        
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
            RuntimeManager.PlayOneShotAttached(soundReference, gameObject);
        }
    }
}