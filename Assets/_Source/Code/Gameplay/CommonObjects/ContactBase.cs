using System;
using JetBrains.Annotations;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    [SelectionBase]
    public abstract class ContactBase : MonoBehaviour
    {
        public Action<ContactBase> OnContact;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetActor(out CharacterBase ctx))
            {
                Contact(other, ctx);
            }
        }

        public virtual void Contact(Collider msg, CharacterBase context)
        {
            ObjectEvents.OnObjectTriggered?.Invoke(this);
            
            OnContact?.Invoke(this);
        }
    }
}