using FMODUnity;
using SurgeEngine.Code.Enemy.States;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EnemyRagdoll : MonoBehaviour
    {
        [Header("Collision")]
        public Rigidbody root;
        public LayerMask collideLayers;

        float timer;
        bool hit = false;

        [Header("Explosion")]
        [SerializeField] private ParticleSystem explosionEffect;
        [SerializeField] private float explosionOffset = 0.5f;
        [SerializeField] private EventReference explosionReference;

        private void Update()
        {
            timer += Time.deltaTime;
            
            if (timer >= 5f)
                Explode();
        }

        public void Explode()
        {
            if (hit || timer < 0.25f)
                return;

            hit = true;

            ParticleSystem particle = Instantiate(explosionEffect, root.position + Vector3.up * explosionOffset, Quaternion.identity);
            Destroy(particle.gameObject, 2.5f);

            RuntimeManager.PlayOneShot(explosionReference, root.position);

            Destroy(gameObject);
        }
    }
}
