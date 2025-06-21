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
            ObjectEvents.OnObjectCollected?.Invoke(this);
            
            OnContact?.Invoke(this);
        }
        
        protected virtual void OnDrawGizmos()
        {
            
        }
        
        protected bool CheckFacing(Vector3 dir)
        {
            float dot = Vector3.Dot(transform.forward, dir);
            Debug.Log(dot);
            return dot > 0.02f;
        }
    }
}