using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Sensors;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateChase : EGState
    {
        public EGStateChase(EnemyBase enemy) : base(enemy)
        {
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            bool hasTarget = sensor.FindVisibleTargets(out var pos);
            if (!hasTarget)
            {
                Debug.DrawLine(transform.position, pos, Color.blue);
            }
            
            Vector3 direction = (pos - transform.position).normalized;
            direction = Vector3.ProjectOnPlane(direction, Vector3.up);
            eggFighter.rb.linearVelocity = direction * eggFighter.chaseSpeed;
                
            Quaternion rotation = Quaternion.LookRotation(pos - transform.position, Vector3.up);
            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

            if (Vector3.Distance(pos, transform.position) < eggFighter.punchRadius)
            {
                stateMachine.SetState<EGStatePunch>();
            }
        }
    }
}