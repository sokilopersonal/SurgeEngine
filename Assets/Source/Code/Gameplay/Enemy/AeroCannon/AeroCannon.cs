using SurgeEngine.Source.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Source.Code.Gameplay.Enemy.AeroCannon.States;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.AeroCannon
{
    public class AeroCannon : EnemyBase, IDamageable
    {
        [SerializeField] private new AeroCannonAnimation animation;
        [SerializeField] private float viewDistance = 10;
        [SerializeField] private LayerMask mask;
        [SerializeField] private float idleTime = 1.5f;
        [SerializeField] private float prepareTime = 1;
        
        public float ViewDistance => viewDistance;
        public LayerMask Mask => mask;
        public float IdleTime => idleTime;
        public float PrepareTime => prepareTime;
        
        public Transform shootPoint;
        public AeroCannonBullet bulletPrefab;
        
        protected override void Awake()
        {
            base.Awake();
            
            animation.Initialize(this);
            
            StateMachine.AddState(new ACStateIdle(this));
            StateMachine.AddState(new ACStatePrepare(this));
            StateMachine.AddState(new ACStateShoot(this));
            
            StateMachine.SetState<ACStateIdle>().SetStartRotation(transform.rotation);
        }

        public void TakeDamage(Component sender)
        {
            OnDied?.Invoke();
            
            View.Destroy();
        }
    }
}