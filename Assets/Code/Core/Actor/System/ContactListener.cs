using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Code.Actor.System
{
    /// <summary>
    /// Listens for collisions
    /// </summary>
    public class ContactListener : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            IPlayerContactable playerContactable = null;

            if (other.TryGetComponent(out ContactBase contactable) || other.TryGetComponent(out playerContactable)) {}
            else if (other.transform.parent != null)
            {
                other.transform.parent.TryGetComponent(out contactable);
                other.transform.parent.TryGetComponent(out playerContactable);
            }

            var col = GetComponent<Collider>();
            contactable?.Contact(col);
            playerContactable?.OnContact(col);
        }
    }
}