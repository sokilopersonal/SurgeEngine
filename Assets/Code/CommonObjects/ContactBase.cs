using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public abstract class ContactBase : MonoBehaviour
    {
        [Header("Collision")]
        [SerializeField] protected Vector3 offset;
        [SerializeField] protected float collisionWidth = 0.3f;
        [SerializeField] protected float collisionHeight = 0.3f;
        [SerializeField] protected float collisionDepth = 0.3f;

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

        protected virtual void OnCollisionContact(Collision msg)
        {
            
        }
        
        protected virtual void OnTriggerContact(Collider msg)
        {
            
        }
        
        protected virtual void Draw()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(Vector3.zero + offset, new Vector3(collisionWidth, collisionHeight, collisionDepth));
        }

        private void OnDrawGizmos()
        {
            Draw();
        }
    }
}