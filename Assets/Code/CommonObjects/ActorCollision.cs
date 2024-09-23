using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ActorCollision : ContactBase
    {
        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Collision");
        }
    }
}