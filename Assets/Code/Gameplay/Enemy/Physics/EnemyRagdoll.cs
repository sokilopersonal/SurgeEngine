using FMODUnity;
using System.Collections.Generic;
using SurgeEngine.Code.CommonObjects.System;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.Enemy
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

        [HideInInspector]
        public float timer;

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
                limb.ragdoll = this;
                limb.rb.mass *= limbMassScale;
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
            
            _eggFighter.View.Destroy();
        }

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
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
