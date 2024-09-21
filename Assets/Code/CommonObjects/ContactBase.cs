using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public abstract class ContactBase : MonoBehaviour
    {
        [SerializeField] protected Vector3 offset;
        [SerializeField] protected float collisionWidth = 2f;
        [SerializeField] protected float collisionHeight = 2f;
        [SerializeField] protected float collisionDepth = 0.5f;

        private void OnCollisionEnter(Collision msg)
        {
            if (ActorContext.Context.gameObject == msg.gameObject)
            {
                OnCollisionContact(msg);
            }
        }

        private void OnTriggerEnter(Collider msg)
        {
            if (ActorContext.Context.gameObject == msg.gameObject)
            {
                OnTriggerContact(msg);
            }
        }

        protected virtual void OnCollisionContact(Collision msg)
        {
            
        }
        
        protected virtual void OnTriggerContact(Collider msg)
        {
            
        }
        
        protected virtual void Draw()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position + offset, new Vector3(collisionWidth, collisionHeight, collisionDepth));
        }

        private void OnDrawGizmos()
        {
            Draw();
        }
    }
}