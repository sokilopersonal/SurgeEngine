using System;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    [SelectionBase]
    public abstract class ContactBase : MonoBehaviour
    {
        public Action<ContactBase> OnContact; 

        private void OnCollisionEnter(Collision msg)
        {
            if (ActorContext.Context.gameObject == msg.transform.parent.gameObject) // Check for ContactCollision instead
            {
                OnCollisionContact(msg);
            }
        }

        private void OnTriggerEnter(Collider msg)
        {
            if (ActorContext.Context.gameObject == msg.transform.parent.gameObject) // Check for ContactCollision instead
            {
                OnTriggerContact(msg);
            }
        }

        public virtual void OnCollisionContact(Collision msg)
        {
            ActorContext.Context.stats.lastContactObject = this;
            
            OnContact?.Invoke(this);
        }

        public virtual void OnTriggerContact(Collider msg)
        {
            ActorContext.Context.stats.lastContactObject = this;
            
            OnContact?.Invoke(this);
        }
        
        protected virtual void Draw()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
        }

        private void OnDrawGizmos()
        {
            Draw();
        }
    }
}