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
    }
}