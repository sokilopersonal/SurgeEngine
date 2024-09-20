using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ActorCollision : ContactBase
    {
        private void Awake()
        {
            var collision = gameObject.AddComponent<BoxCollider>();
            collision.center = offset;
            collision.size = new Vector3(collisionWidth, collisionHeight, collisionDepth);
            collision.isTrigger = false;

            gameObject.layer = LayerMask.NameToLayer("Collision");
        }
    }
}