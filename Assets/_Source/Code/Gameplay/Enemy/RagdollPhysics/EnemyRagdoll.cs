using System.Collections.Generic;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Gameplay.Enemy.EggFighter;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.RagdollPhysics
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
        public LayerMask collideLayers;

        private float timer;
        public float Timer => timer;

        private bool _hit;
        private bool _isInRagdoll;

        [Header("Lifetime")]
        public float minimumLifeTime = 0.25f;
        public float maximumLifeTime = 4f;

        private EggFighter.EggFighter _eggFighter;

        private void Start()
        {
            _eggFighter = GetComponentInParent<EggFighter.EggFighter>();
            
            foreach (EnemyRagdollLimb limb in limbs)
            {
                limb.SetRagdoll(this);
                limb.Rigidbody.mass *= limbMassScale;
            }
        }

        public void Ragdoll(Vector3 force = new Vector3(), ForceMode mode = ForceMode.VelocityChange)
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
            
            timer += Time.deltaTime;
            
            if (timer > maximumLifeTime)
                Explode();
        }

        public void Explode()
        {
            if (_hit)
                return;

            _hit = true;

            (_eggFighter.View as EGView)?.Destroy();
        }

        public void Load()
        {
            _isInRagdoll = false;
            _hit = false;
            timer = 0;

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
