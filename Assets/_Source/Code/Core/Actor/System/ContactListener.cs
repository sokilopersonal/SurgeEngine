using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    /// <summary>
    /// Listens for collisions
    /// </summary>
    public class ContactListener : ActorComponent
    {
        private void OnTriggerEnter(Collider other)
        {
            IPlayerContactable playerContactable = null;

            if (other.TryGetComponent(out ContactBase contactable) || other.TryGetComponent(out playerContactable)) {}
            else if (other.transform.parent != null)
            {
                other.transform.TryGetComponentInParent(out contactable);
                other.transform.TryGetComponentInParent(out playerContactable);
            }

            var col = GetComponent<Collider>();
            contactable?.Contact(col, Actor);
            playerContactable?.OnContact(col);
        }
    }
}