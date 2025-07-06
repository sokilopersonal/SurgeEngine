using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateIdle : EGState
    {
        public EGStateIdle(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            var agent = eggFighter.Agent;
            agent.velocity = Vector3.zero;

            var path = new NavMeshPath();
            Debug.DrawRay(agent.destination, Vector3.up, Color.red, 2);
            if (sensor.FindVisibleTargets(out var pos))
            {
                if (!agent.hasPath)
                    stateMachine.SetState<EGStateChase>();
                else
                {
                    agent.CalculatePath(pos, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                        stateMachine.SetState<EGStateChase>();
                }
            }
        }
    }
}