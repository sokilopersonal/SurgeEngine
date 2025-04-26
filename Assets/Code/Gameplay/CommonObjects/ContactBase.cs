using System;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    [SelectionBase]
    public abstract class ContactBase : Entity
    {
        public Action<ContactBase> OnContact;
        public Action<ContactBase> OnDetach;

        public virtual void Contact(Collider msg)
        {
            ActorBase context = ActorContext.Context;
            context.stats.lastContactObject = this;
            
            ObjectEvents.OnObjectCollected?.Invoke(this);
            
            OnContact?.Invoke(this);
        }
        
        protected virtual void OnDrawGizmos()
        {
            
        }
    }
}