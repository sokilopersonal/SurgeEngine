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

        private void Awake()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetActor(out ActorBase ctx))
            {
                Contact(other, ctx);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetActor(out ActorBase ctx))
            {
                OnDetach?.Invoke(this);
            }
        }

        public virtual void Contact(Collider msg, ActorBase context)
        {
            ObjectEvents.OnObjectTriggered?.Invoke(this);
            
            OnContact?.Invoke(this);
        }
        
        protected virtual void OnDrawGizmos()
        {
            
        }
    }
}