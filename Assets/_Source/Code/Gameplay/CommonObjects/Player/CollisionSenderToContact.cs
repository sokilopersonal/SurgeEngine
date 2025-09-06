using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Player
{
    public class CollisionSenderToContact : MonoBehaviour
    {
        private StageObject contact;

        private void Awake()
        {
            contact = GetComponentInParent<StageObject>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetCharacter(out CharacterBase ctx))
            {
                contact.Contact(other, ctx);
            }
        }
    }
}