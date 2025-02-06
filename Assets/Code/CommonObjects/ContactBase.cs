using System;
using SurgeEngine.Code.ActorSystem;
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
            Actor context = ActorContext.Context;
            context.stats.lastContactObject = this;
            
            OnContact?.Invoke(this);
        }
        
        protected virtual void OnDrawGizmos()
        {
            
        }
    }
}