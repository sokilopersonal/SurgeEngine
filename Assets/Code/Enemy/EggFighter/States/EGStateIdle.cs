using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.EggFighter.States
{
    public class EGStateIdle : EGState
    {
        private float _stayTimer;
        
        public EGStateIdle(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stayTimer = 0f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            eggFighter.rb.linearVelocity = Vector3.zero;

            if (eggFighter.patrolTime == 0) return;

            Actor context = ActorContext.Context;
            if (Vector3.Distance(context.transform.position, transform.position) < eggFighter.findDistance)
            {
                eggFighter.stateMachine.SetState<EGStateChase>();
            }

            if (_stayTimer < eggFighter.patrolTime)
            {
                _stayTimer += Time.deltaTime;
            }
            else
            {
                // Vector3 point = eggFighter.stateMachine.GetState<EGStatePatrol>().GetRandomPoint();
                // float angleDiff = transform.rotation.eulerAngles.y - Quaternion.LookRotation(point - transform.position).eulerAngles.y;
                //
                // angleDiff = 130;
                //
                // if (Mathf.Abs(angleDiff) >= 120)
                // {
                //     eggFighter.stateMachine.SetState<EGStateTurn>();
                // }
                // else
                // {
                //     eggFighter.stateMachine.SetState<EGStatePatrol>(0.2f).SetNewPatrolPoint(point);
                // }

                eggFighter.stateMachine.SetState<EGStatePatrol>();
            }
        }
    }
}