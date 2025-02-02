using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.Enemy
{
    public class EnemyRagdoll : MonoBehaviour
    {
        [Header("Collision")] 
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private List<Collider> disableWhenRagdoll;
        [SerializeField] private UnityEvent onRagdoll;
        [SerializeField] private List<EnemyRagdollLimb> limbs;
        [SerializeField] private float limbMassScale = 1f;
        public LayerMask collideLayers;

        [HideInInspector]
        public float timer;

        private bool hit;
        private bool ragdoll;

        [Header("Lifetime")]
        public float minimumLifeTime = 0.25f;
        public float maximumLifeTime = 4f;

        private EggFighter _eggFighter;

        private void Start()
        {
            _eggFighter = GetComponentInParent<EggFighter>();
            
            foreach (EnemyRagdollLimb limb in limbs)
            {
                limb.ragdoll = this;
                limb.rb.mass *= limbMassScale;
            }
        }

        public void Ragdoll(Vector3 force = new Vector3(), ForceMode mode = ForceMode.VelocityChange)
        {
            if (ragdoll)
                return;

            ragdoll = true;
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
            if (!ragdoll)
                return;
            
            timer += Time.deltaTime;
            
            if (timer > maximumLifeTime)
                Explode();
        }

        public void Explode()
        {
            if (hit)
                return;

            hit = true;
            
            _eggFighter.View.Destroy();
        }
    }
}
