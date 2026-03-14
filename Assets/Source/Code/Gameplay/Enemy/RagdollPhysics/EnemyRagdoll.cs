using System.Collections.Generic;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.RagdollPhysics
{
    public class EnemyRagdoll : MonoBehaviour, IPointMarkerLoader
    {
        [Header("Collision")] 
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private List<Collider> disableWhenRagdoll;
        [SerializeField] private UnityEvent onRagdoll;
        [SerializeField] private UnityEvent onPointMarkerLoad;
        [SerializeField] private List<EnemyRagdollLimb> limbs;
        [SerializeField] private float limbMassScale = 1f;
        [SerializeField] private LayerMask collideLayers;
        public LayerMask CollideLayers => collideLayers;

        private float _timer;
        public float Timer => _timer;

        private bool _hit;
        private bool _isInRagdoll;

        [Header("Lifetime")]
        [SerializeField] private float minimumLifeTime = 0.25f;
        [SerializeField] private float maximumLifeTime = 4f;
        public float MinimumLifeTime => minimumLifeTime;
        public float MaximumLifeTime => maximumLifeTime;

        private void Start()
        {
            foreach (EnemyRagdollLimb limb in limbs)
            {
                limb.SetRagdoll(this);
                limb.Rigidbody.mass *= limbMassScale;
            }
        }

        public void Ragdoll(Vector3 force = new(), ForceMode mode = ForceMode.VelocityChange)
        {
            if (_isInRagdoll)
                return;

            _isInRagdoll = true;
            meshRenderer.updateWhenOffscreen = true;

            foreach (Collider disableCol in disableWhenRagdoll)
            {
                disableCol.enabled = false;
            }

            foreach (EnemyRagdollLimb limb in limbs)
            {
                limb.SetActive(true);
                limb.AddForce(force, mode);
            }

            onRagdoll.Invoke();
        }

        private void Update()
        {
            if (!_isInRagdoll)
                return;
            
            _timer += Time.deltaTime;
            
            if (_timer > MaximumLifeTime)
                Explode();
        }

        public void Explode()
        {
            if (_hit)
                return;

            _hit = true;
            OnExplode();
        }

        protected virtual void OnExplode() { }

        public void Load()
        {
            _isInRagdoll = false;
            _hit = false;
            _timer = 0;

            foreach (var limb in limbs)
            {
                limb.SetActive(false);
            }
            
            foreach (Collider disableCol in disableWhenRagdoll)
            {
                disableCol.enabled = true;
            }
            
            onPointMarkerLoad.Invoke();
        }
    }
}
