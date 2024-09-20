using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ActorTrigger : ContactBase
    {
        private void Awake()
        {
            var collision = gameObject.AddComponent<BoxCollider>();
            collision.size = new Vector3(collisionWidth, collisionHeight, collisionDepth);
            collision.isTrigger = true;
        }
    }
}