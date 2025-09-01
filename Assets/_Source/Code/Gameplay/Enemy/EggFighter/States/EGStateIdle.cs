using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.Enemy.Base;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateIdle : EGState
    {
        private float _stayTimer;
        
        public EGStateIdle(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            bool hasTarget = sensor.FindVisibleTarget(out var pos, out var character);
            Utility.TickTimer(ref _stayTimer, _stayTimer, false);

            if (_stayTimer <= 0 && eggFighter.FollowPlayer)
            {
                var agent = eggFighter.Agent;
                agent.velocity = Vector3.zero;

                var path = new NavMeshPath();
                if (hasTarget)
                {
                    if (!agent.hasPath)
                        StateMachine.SetState<EGStateChase>();
                    else
                    {
                        agent.CalculatePath(pos, path);
                        if (path.status == NavMeshPathStatus.PathComplete && !character.Flags.HasFlag(FlagType.Invincible))
                            StateMachine.SetState<EGStateChase>();
                    }
                }
            }

            if (!eggFighter.FollowPlayer)
            {
                if (Vector3.Distance(transform.position, eggFighter.Character.transform.position) <=
                    eggFighter.PunchRadius)
                {
                    StateMachine.SetState<EGStatePunch>();
                }
            }
        }
        
        public void SetStayTimer(float timer) => _stayTimer = timer;
    }
}