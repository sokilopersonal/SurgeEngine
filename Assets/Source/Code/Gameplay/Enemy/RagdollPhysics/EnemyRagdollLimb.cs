using System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.RagdollPhysics
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class EnemyRagdollLimb : MonoBehaviour
    {
        private EnemyRagdoll _ragdoll;
        private Rigidbody _rb;
        public Rigidbody Rigidbody => _rb;

        private Collider _col;
        private bool _active;
        
        private void Awake()
        {
            _col = GetComponent<Collider>();
            _rb = GetComponent<Rigidbody>();
            SetActive(_active);
        }

        public void SetActive(bool active)
        {
            _active = active;
            _col.enabled = _active;
            _rb.isKinematic = !_active;
            _rb.interpolation = RigidbodyInterpolation.None;
        }
        
        public void AddForce(Vector3 force, ForceMode mode)
        {
            if (!_active)
                return;
            
            _rb.AddForce(force, mode);
        }

        public void SetRagdoll(EnemyRagdoll ragdoll)
        {
            _ragdoll = ragdoll;
        }
        
        public void OnCollisionEnter(Collision collision)
        {
            if (_active && _ragdoll != null && _ragdoll.Timer > _ragdoll.MinimumLifeTime && _ragdoll.CollideLayers == (_ragdoll.CollideLayers | (1 << collision.gameObject.layer)))
                _ragdoll.Explode();
        }
        
        public void OnCollisionStay(Collision collision)
        {
            if (_active && _ragdoll != null && _ragdoll.Timer > _ragdoll.MinimumLifeTime && _ragdoll.CollideLayers == (_ragdoll.CollideLayers | (1 << collision.gameObject.layer)))
                _ragdoll.Explode();
        }

        public void ResetVelocity()
        {
            _rb.linearVelocity = Vector3.zero;
        }
    }
}
