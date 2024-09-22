using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ActorTrigger : ContactBase
    {
       protected virtual void Awake()
        {
            var collision = gameObject.AddComponent<BoxCollider>();
            collision.center = offset;
            collision.size = new Vector3(collisionWidth, collisionHeight, collisionDepth);
            collision.isTrigger = true;

            gameObject.layer = LayerMask.NameToLayer("Trigger");
        }
    }
}