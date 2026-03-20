using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter.States
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

            var character = EggFighter.Character;
            if (!EggFighter.FollowPlayer || !Agent.enabled)
            {
                if (Vector3.Distance(Transform.position, EggFighter.Character.transform.position) <=
                    EggFighter.PunchRadius && !character.Flags.HasFlag(FlagType.Invincible))
                {
                    StateMachine.SetState<EGStatePunch>();
                    return;
                }
            }

            if (!Agent.enabled)
                return;

            bool hasTarget = Sensor.FindVisibleTarget(out var pos, out _);
            Utility.TickTimer(ref _stayTimer, _stayTimer, false);
            if (_stayTimer <= 0 && EggFighter.FollowPlayer)
            {
                Agent.velocity = Vector3.zero;

                var path = new NavMeshPath();
                if (hasTarget)
                {
                    if (!Agent.hasPath)
                        StateMachine.SetState<EGStateChase>();
                    else
                    {
                        Agent.CalculatePath(pos, path);
                        if (path.status == NavMeshPathStatus.PathComplete && !character.Flags.HasFlag(FlagType.Invincible))
                            StateMachine.SetState<EGStateChase>();
                    }
                }
            }
        }
        
        public void SetStayTimer(float timer) => _stayTimer = timer;
    }
}