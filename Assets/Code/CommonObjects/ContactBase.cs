using System;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    [SelectionBase]
    public abstract class ContactBase : MonoBehaviour
    {
        public Action<ContactBase> OnContact;
        public Action<ContactBase> OnDetach;
        public bool isChangePath;

        private void OnCollisionEnter(Collision msg)
        {
            if (IsContact(msg))
            {
                OnCollisionContact(msg);
            }
        }

        private void OnTriggerEnter(Collider msg)
        {
            if (IsContact(msg))
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
            var context = ActorContext.Context;
            context.stats.lastContactObject = this;

            if (isChangePath)
            {
                context.kinematics.SetPath(null);
            }
            
            OnContact?.Invoke(this);
        }

        public virtual void OnTriggerDetach(Collider msg)
        {
            OnDetach?.Invoke(this);
        }

        internal bool IsContact(Collision msg)
        {
            return ActorContext.Context.gameObject == msg.transform.parent.gameObject;
        }

        internal bool IsContact(Collider msg)
        {
            return ActorContext.Context.gameObject == msg.transform.parent.gameObject;
        }

        protected virtual void Draw()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
        }

        private void OnDrawGizmosSelected()
        {
            Draw();
        }
    }
}