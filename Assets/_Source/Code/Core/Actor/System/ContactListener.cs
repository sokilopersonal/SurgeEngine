using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    /// <summary>
    /// Listens for collisions
    /// </summary>
    public class ContactListener : CharacterComponent
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IPlayerContactable playerContactable)) {}
            else if (other.transform.parent != null)
            {
                other.transform.TryGetComponentInParent(out playerContactable);
            }
        
            var col = GetComponent<Collider>();
            playerContactable?.OnContact(col);
        }
    }
}