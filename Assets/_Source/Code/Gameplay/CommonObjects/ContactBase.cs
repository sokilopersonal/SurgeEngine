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

        private bool _canBeTriggered;

        private void Awake()
        {
            _canBeTriggered = true;
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

        public virtual void Contact([NotNull] Collider msg, ActorBase context)
        {
            ObjectEvents.OnObjectCollected?.Invoke(this);
            
            OnContact?.Invoke(this);
        }
        
        protected virtual void OnDrawGizmos()
        {
            
        }
    }
}