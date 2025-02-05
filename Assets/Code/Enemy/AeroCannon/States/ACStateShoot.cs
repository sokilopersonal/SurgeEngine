
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.AeroCannon.States
{
    public class ACStateShoot : ACState
    {
        private float _timer;
        
        public ACStateShoot(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = config.prepareTime;

            if (IsInSight(out var target))
            {
                Vector3 direction = (target.position - Vector3.up * 0.5f) - transform.position;
                direction.Normalize();

                var bullet = Object.Instantiate(aeroCannon.bulletPrefab, aeroCannon.shootPoint.position, Quaternion.identity);
                bullet.SetDirection(direction);
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Common.TickTimer(ref _timer, config.prepareTime))
            {
                stateMachine.SetState<ACStateIdle>();
            }
        }
    }
}