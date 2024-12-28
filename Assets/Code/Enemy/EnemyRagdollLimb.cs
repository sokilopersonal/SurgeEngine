using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EnemyRagdollLimb : MonoBehaviour
    {
        public EnemyRagdoll ragdoll;
        public void OnCollisionEnter(Collision collision)
        {
            if (ragdoll.timer >= ragdoll.minimumLifeTime && ragdoll.collideLayers == (ragdoll.collideLayers | (1 << collision.gameObject.layer)))
                ragdoll.Explode();
        }
        public void OnCollisionStay(Collision collision)
        {
            if (ragdoll.timer < ragdoll.minimumLifeTime)
                return;

            if (ragdoll.collideLayers == (ragdoll.collideLayers | (1 << collision.gameObject.layer)))
                ragdoll.Explode();
        }
    }
}
