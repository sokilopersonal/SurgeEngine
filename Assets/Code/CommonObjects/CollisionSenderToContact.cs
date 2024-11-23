using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class CollisionSenderToContact : MonoBehaviour
    {
        private ContactBase contact;

        private void Awake()
        {
            contact = GetComponentInParent<ContactBase>();
        }

        private void OnCollisionEnter(Collision msg)
        {
            if (contact.IsContact(msg))
            {
                contact.OnCollisionContact(msg);
            }
        }

        private void OnTriggerEnter(Collider msg)
        {
            if (contact.IsContact(msg))
            {
                contact.OnTriggerContact(msg);
            }
        }

        private void OnTriggerExit(Collider msg)
        {
            if (contact.IsContact(msg))
            {
                contact.OnTriggerDetach(msg);
            }
        }
    }
}