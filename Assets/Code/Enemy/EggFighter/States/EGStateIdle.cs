using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStateIdle : EGState
    {
        private float _stayTimer;
        
        public EGStateIdle(EggFighter eggFighter, Transform transform, Rigidbody rb) : base(eggFighter, transform, rb)
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
            
            Rb.linearVelocity = Vector3.zero;

            if (eggFighter.patrolTime == 0) return;

            var context = ActorContext.Context;
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