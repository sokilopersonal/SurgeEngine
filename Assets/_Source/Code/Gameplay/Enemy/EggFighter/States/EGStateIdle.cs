using SurgeEngine.Code.Gameplay.Enemy.Base;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
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
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            Utility.TickTimer(ref _stayTimer, _stayTimer, false);

            if (_stayTimer <= 0)
            {
                var agent = eggFighter.Agent;
                agent.velocity = Vector3.zero;

                var path = new NavMeshPath();
                if (sensor.FindVisibleTarget(out var pos, out _))
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
        
        public void SetStayTimer(float timer) => _stayTimer = timer;
    }
}