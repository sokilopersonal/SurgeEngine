using SurgeEngine._Source.Code.Gameplay.Enemy.Base;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.AeroCannon.States
{
    public class ACStateShoot : ACState
    {
        public ACStateShoot(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            timer = 1;

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

            if (Utility.TickTimer(ref timer, 1, false))
            {
                StateMachine.SetState<ACStateIdle>();
            }
        }
    }
}