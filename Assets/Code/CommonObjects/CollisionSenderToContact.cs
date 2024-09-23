using SurgeEngine.Code.ActorSystem;
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
            if (ActorContext.Context.gameObject == msg.transform.parent.gameObject)
            {
                contact.OnCollisionContact(msg);
            }
        }

        private void OnTriggerEnter(Collider msg)
        {
            if (ActorContext.Context.gameObject == msg.transform.parent.gameObject)
            {
                contact.OnTriggerContact(msg);
            }
        }
    }
}