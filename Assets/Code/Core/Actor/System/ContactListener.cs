using System;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    /// <summary>
    /// Listens for collisions
    /// </summary>
    public class ContactListener : MonoBehaviour
    {
        private ActorBase _actor;

        private void Awake()
        {
            _actor = GetComponentInParent<ActorBase>();
        }

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
            contactable?.Contact(col, _actor);
            playerContactable?.OnContact(col);
        }
    }
}