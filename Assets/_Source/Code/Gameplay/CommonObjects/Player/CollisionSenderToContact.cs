using System;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetActor(out CharacterBase ctx))
            {
                contact.Contact(other, ctx);
            }
        }
    }
}