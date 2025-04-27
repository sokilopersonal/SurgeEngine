using FMODUnity;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.Physics;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.Base
{
    public class EnemyView : MonoBehaviour, IEnemyComponent, IPointMarkerLoader
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
            
            ObjectEvents.OnEnemyDied?.Invoke(enemyBase);
            Stage.Instance.data.AddScore(300);
            
            transform.parent.gameObject.SetActive(false);
        }

        public virtual void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            transform.parent.gameObject.SetActive(true);
        }
    }
}