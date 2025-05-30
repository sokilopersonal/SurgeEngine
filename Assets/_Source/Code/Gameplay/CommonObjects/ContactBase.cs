using System;
using JetBrains.Annotations;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    [SelectionBase]
    public abstract class ContactBase : Entity
    {
        public Action<ContactBase> OnContact;
        public Action<ContactBase> OnDetach;

        public virtual void Contact([NotNull] Collider msg, [CanBeNull] ActorBase context)
        {
            if (context) context.Stats.lastContactObject = this;
            
            ObjectEvents.OnObjectCollected?.Invoke(this);
            
            OnContact?.Invoke(this);
        }
        
        protected virtual void OnDrawGizmos()
        {
            
        }
    }
}