using SurgeEngine._Source.Code.Gameplay.Enemy.Base;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.AeroCannon.States
{
    public class ACStateIdle : ACState
    {
        private Quaternion _startRotation;
        
        public ACStateIdle(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            timer = aeroCannon.IdleTime;
        }

        public override void OnTick(float dt)
        {
            bool inSight = IsInSight(out var target);
            if (Utility.TickTimer(ref timer, aeroCannon.IdleTime, false))
            {
                if (inSight)
                {
                    StateMachine.SetState<ACStatePrepare>();
                }
            }

            if (inSight)
            {
                Vector3 direction = target.position - transform.position;
                direction.Normalize();
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _startRotation, 2f * Time.deltaTime);
            }
        }
        
        public void SetStartRotation(Quaternion startRotation) => _startRotation = startRotation;
    }
}