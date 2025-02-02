using FMODUnity;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EnemyView : MonoBehaviour, IEnemyComponent
    {
        public EnemyBase enemyBase { get; set; }
        
        [Header("Destroy")]
        [SerializeField] private Transform explosionPoint;
        [SerializeField] private ParticleSystem explosionEffect;
        [SerializeField] private DestroyedPiece destroyedPiece;
        [SerializeField] private float explosionOffset = 0.25f;
        [SerializeField] protected EventReference explosionReference;
        [SerializeField] protected EventReference metalHitReference;

        private void Update()
        {
            ViewTick();
        }

        protected virtual void ViewTick()
        {
            
        }
        
        public void Destroy()
        {
            ParticleSystem particle = Instantiate(explosionEffect, explosionPoint.position + Vector3.up * explosionOffset, Quaternion.identity);
            Destroy(particle.gameObject, 2.5f);
            
            if (destroyedPiece != null)
            {
                var pieces = Instantiate(destroyedPiece, explosionPoint.position, transform.rotation, null);
                pieces.ApplyExplosionForce(1000f, explosionPoint.position, 2f);
            }
            
            RuntimeManager.PlayOneShot(explosionReference, explosionPoint.position);
            
            Destroy(enemyBase.gameObject);
        }
    }
}