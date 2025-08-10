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
        public Action<ContactBase> OnDetach;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetActor(out CharacterBase ctx))
            {
                Contact(other, ctx);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetActor(out CharacterBase ctx))
            {
                OnDetach?.Invoke(this);
            }
        }

        public virtual void Contact(Collider msg, CharacterBase context)
        {
            ObjectEvents.OnObjectTriggered?.Invoke(this);
            
            OnContact?.Invoke(this);
        }
    }
}