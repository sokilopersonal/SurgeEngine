using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Player
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