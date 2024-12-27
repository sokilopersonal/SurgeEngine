using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EnemyRagdollLimb : MonoBehaviour
    {
        public EnemyRagdoll ragdoll;
        int enemyLayer = 69;
        private void Awake()
        {
            enemyLayer = LayerMask.NameToLayer("Enemy");
        }
        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == enemyLayer)
                ragdoll.ExplodeImmediate();
            else if (ragdoll.timer >= ragdoll.minimumLifeTime && ragdoll.collideLayers == (ragdoll.collideLayers | (1 << collision.gameObject.layer)))
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
