using FMODUnity;
using SurgeEngine.Code.Enemy.States;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EnemyRagdoll : MonoBehaviour
    {
        [Header("Collision")]
        public Rigidbody root;
        public float collideRadius;
        public LayerMask collideLayers;

        float timer;
        bool hit = false;

        [Header("Explosion")]
        [SerializeField] private ParticleSystem explosionEffect;
        [SerializeField] private float explosionOffset = 0.5f;
        [SerializeField] private EventReference explosionReference;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(root.position, collideRadius);
        }

        private void Update()
        {
            timer += Time.deltaTime;
        }

        private void Explode()
        {
            if (hit)
                return;

            hit = true;

            ParticleSystem particle = Instantiate(explosionEffect, root.position + Vector3.up * explosionOffset, Quaternion.identity);
            Destroy(particle.gameObject, 2.5f);

            RuntimeManager.PlayOneShot(explosionReference, root.position);

            Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            if (timer < 0.25f)
                return;
            
            if (hit || !Physics.CheckSphere(root.position, collideRadius, collideLayers))
                return;

            Explode();
        }
    }
}
