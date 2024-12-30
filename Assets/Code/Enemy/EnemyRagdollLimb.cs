using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.Enemy
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class EnemyRagdollLimb : MonoBehaviour
    {
        [HideInInspector]
        public EnemyRagdoll ragdoll;

        [HideInInspector]
        public Rigidbody rb;

        private Collider col;
        private bool active = false;
        private void Awake()
        {
            col = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();
            SetActive(active);
        }
        public void SetActive(bool _set)
        {
            active = _set;
            col.enabled = active;
            rb.isKinematic = !active;
        }
        public void AddForce(Vector3 force, ForceMode mode)
        {
            if (!active)
                return;
            rb.AddForce(force, mode);
        }
        public void OnCollisionEnter(Collision collision)
        {
            if (active && ragdoll != null && ragdoll.timer > ragdoll.minimumLifeTime && ragdoll.collideLayers == (ragdoll.collideLayers | (1 << collision.gameObject.layer)))
                ragdoll.Explode();
        }
        public void OnCollisionStay(Collision collision)
        {
            if (active && ragdoll != null && ragdoll.timer > ragdoll.minimumLifeTime && ragdoll.collideLayers == (ragdoll.collideLayers | (1 << collision.gameObject.layer)))
                ragdoll.Explode();
        }
    }
}
