using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public abstract class ContactBase : MonoBehaviour
    {
        [SerializeField] protected float collisionWidth = 10f;
        [SerializeField] protected float collisionHeight = 10f;
        [SerializeField] protected float collisionDepth = 10f;

        private void OnCollisionEnter(Collision other)
        {
            if (ActorContext.Context.ID == other.gameObject.GetInstanceID())
            {
                Debug.Log("Collision enter from Sonic");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ActorContext.Context.ID == other.gameObject.GetInstanceID())
            {
                Debug.Log("Trigger enter from Sonic");
            }
        }

        protected virtual void Draw()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(collisionWidth, collisionHeight, collisionDepth));
        }

        private void OnDrawGizmos()
        {
            Draw();
        }
    }
}