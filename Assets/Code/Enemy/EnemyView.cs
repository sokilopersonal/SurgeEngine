using System;
using FMODUnity;
using SurgeEngine.Code.Enemy.States;
using UnityEngine;

namespace SurgeEngine.Code.StateMachine
{
    public class EnemyView : MonoBehaviour, IEnemyComponent
    {
        public EnemyBase enemyBase { get; set; }
        
        public CapsuleCollider eCollider;
        [SerializeField] private ParticleSystem explosionEffect;
        [SerializeField] private float explosionOffset = 0.5f;
        [SerializeField] protected EventReference explosionReference;
        [SerializeField] protected EventReference metalHitReference;

        private void Update()
        {
            ViewTick();
        }

        protected virtual void ViewTick()
        {
            
        }

        protected bool IsAbleExcludePlayer()
        {
            return enemyBase.CanBeDamaged();
        }
        
        public void Destroy()
        {
            ParticleSystem particle = Instantiate(explosionEffect, transform.position + Vector3.up * explosionOffset, Quaternion.identity);
            Destroy(particle.gameObject, 2.5f);
            
            RuntimeManager.PlayOneShot(explosionReference, transform.position);
            
            Destroy(gameObject);
        }
    }
}