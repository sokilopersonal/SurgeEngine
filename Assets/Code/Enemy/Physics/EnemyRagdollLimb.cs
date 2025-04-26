using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class EnemyRagdollLimb : MonoBehaviour
    {
        [HideInInspector]
        public EnemyRagdoll ragdoll;

        [HideInInspector]
        public Rigidbody rb;

        private Collider _col;
        private bool _active;
        
        private void Awake()
        {
            _col = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();
            SetActive(_active);
        }
        
        public void SetActive(bool set)
        {
            _active = set;
            _col.enabled = _active;
            rb.isKinematic = !_active;
        }
        
        public void AddForce(Vector3 force, ForceMode mode)
        {
            if (!_active)
                return;
            rb.AddForce(force, mode);
        }
        
        public void OnCollisionEnter(Collision collision)
        {
            if (_active && ragdoll != null && ragdoll.timer > ragdoll.minimumLifeTime && ragdoll.collideLayers == (ragdoll.collideLayers | (1 << collision.gameObject.layer)))
                ragdoll.Explode();
        }
        
        public void OnCollisionStay(Collision collision)
        {
            if (_active && ragdoll != null && ragdoll.timer > ragdoll.minimumLifeTime && ragdoll.collideLayers == (ragdoll.collideLayers | (1 << collision.gameObject.layer)))
                ragdoll.Explode();
        }

        public void ResetVelocity()
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
