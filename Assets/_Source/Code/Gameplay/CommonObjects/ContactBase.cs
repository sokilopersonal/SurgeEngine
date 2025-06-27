using System;
using JetBrains.Annotations;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    [SelectionBase]
    public abstract class ContactBase : Entity
    {
        public Action<ContactBase> OnContact;
        public Action<ContactBase> OnDetach;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetActor(out ActorBase ctx))
            {
                Contact(other, ctx);
            }
        }

        public virtual void Contact([NotNull] Collider msg, [CanBeNull] ActorBase context)
        {
            ObjectEvents.OnObjectCollected?.Invoke(this);
            
            OnContact?.Invoke(this);
        }
        
        protected virtual void OnDrawGizmos()
        {
            
        }
    }
}