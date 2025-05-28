using SurgeEngine.Code.Gameplay.Enemy.Base;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.AeroCannon.States
{
    public class ACStatePrepare : ACState
    {
        public ACStatePrepare(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            timer = aeroCannon.PrepareTime;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (IsInSight(out var target))
            {
                Vector3 direction = target.position - transform.position;
                direction.Normalize();
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 8f * Time.deltaTime);
                
                if (Utility.TickTimer(ref timer, aeroCannon.PrepareTime, false))
                {
                    stateMachine.SetState<ACStateShoot>();
                }
            }
            else
            {
                stateMachine.SetState<ACStateIdle>();
            }
        }
    }
}