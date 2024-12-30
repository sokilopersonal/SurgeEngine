using FMODUnity;
using SurgeEngine.Code.Enemy.States;
using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EnemyRagdoll : MonoBehaviour
    {
        [Header("Collision")]
        public SkinnedMeshRenderer meshRenderer;
        public GameObject mainObject;
        public List<Collider> disableWhenRagdoll;
        public List<EnemyRagdollLimb> limbs;
        public float limbMassScale = 1f;
        public LayerMask collideLayers;

        [HideInInspector]
        public float timer;

        private bool hit;
        private bool ragdoll = false;

        [Header("Explosion")]
        public Transform explosionPoint;
        public float minimumLifeTime = 0.25f;
        public float maximumLifeTime = 4f;
        [SerializeField] private ParticleSystem explosionEffect;
        [SerializeField] private float explosionOffset = 0.5f;
        [SerializeField] private EventReference explosionReference;

        private void Start()
        {
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

            ParticleSystem particle = Instantiate(explosionEffect, explosionPoint.position + Vector3.up * explosionOffset, Quaternion.identity);
            Destroy(particle.gameObject, 2.5f);

            RuntimeManager.PlayOneShot(explosionReference, explosionPoint.position);

            Destroy(mainObject);
        }
    }
}
