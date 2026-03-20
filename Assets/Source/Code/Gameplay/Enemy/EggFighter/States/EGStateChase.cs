using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateChase : EGState
    {
        public EGStateChase(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            EggFighter.Animation.OnAnimatorMoveEvent += OnAnimatorMove;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            EggFighter.Animation.OnAnimatorMoveEvent -= OnAnimatorMove;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            bool hasTarget = Sensor.FindVisibleTarget(out var pos, out var character);
            if (!hasTarget)
            {
                Debug.DrawLine(Transform.position, pos, Color.blue);
            }
            
            Agent.SetDestination(pos);
            if (Agent.remainingDistance < Agent.stoppingDistance)
            {
                Agent.velocity = Vector3.zero;
                StateMachine.SetState<EGStateIdle>();
            }
            
            if (Vector3.Distance(pos, Transform.position) < EggFighter.PunchRadius)
            {
                if (hasTarget && !character.Life.IsDead && !character.Flags.HasFlag(FlagType.Invincible))
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
            if (!Agent.enabled || Agent.remainingDistance < Agent.stoppingDistance) return;
            
            var rootPos = obj.rootPosition;
            rootPos.y = Agent.nextPosition.y;
            Transform.position = rootPos;
            Agent.nextPosition = rootPos;
        }
    }
}