using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Gameplay.Enemy.AeroCannon.States;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.AeroCannon
{
    public class AeroCannon : EnemyBase, IDamageable
    {
        [SerializeField] private AeroCannonConfig config;
        
        public Transform shootPoint;
        public AeroCannonBullet bulletPrefab;
        
        protected override void Awake()
        {
            base.Awake();
            
            AddConfig(config);
            
            stateMachine.AddState(new ACStateIdle(this));
            stateMachine.AddState(new ACStatePrepare(this));
            stateMachine.AddState(new ACStateShoot(this));
            
            stateMachine.SetState<ACStateIdle>().SetStartRotation(transform.rotation);
        }

        public void TakeDamage(Entity sender, float damage)
        {
            view.Destroy();
        }
    }
}