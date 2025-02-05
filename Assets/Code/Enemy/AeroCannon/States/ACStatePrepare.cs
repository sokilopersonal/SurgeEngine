using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.AeroCannon.States
{
    public class ACStatePrepare : ACState
    {
        private float _timer;
        
        public ACStatePrepare(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = config.prepareTime;
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
                
                if (Common.TickTimer(ref _timer, config.prepareTime, false))
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