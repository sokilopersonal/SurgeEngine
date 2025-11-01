using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateChase : EGState
    {
        public EGStateChase(EnemyBase enemy) : base(enemy)
        {
            eggFighter.Animation.OnAnimatorMoveEvent += OnAnimatorMove;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            bool hasTarget = sensor.FindVisibleTarget(out var pos, out var character);
            if (!hasTarget)
            {
                Debug.DrawLine(transform.position, pos, Color.blue);
            }

            var agent = eggFighter.Agent;
            agent.SetDestination(pos);
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                agent.velocity = Vector3.zero;
                StateMachine.SetState<EGStateIdle>();
            }
            
            if (Vector3.Distance(pos, transform.position) < eggFighter.PunchRadius)
            {
                if (hasTarget && !character.Life.IsDead)
                {
                    StateMachine.SetState<EGStatePunch>();
                }
                else
                {
                    StateMachine.SetState<EGStateIdle>();
                }
            }
        }

        private void OnAnimatorMove(Animator obj)
        {
            var agent = eggFighter.Agent;
            if (!agent.enabled || agent.remainingDistance < agent.stoppingDistance) return;
            
            var rootPos = obj.rootPosition;
            rootPos.y = agent.nextPosition.y;
            transform.position = rootPos;
            agent.nextPosition = rootPos;
        }
    }
}