using SurgeEngine.Code.Core.Actor.System;
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

            ActorBase context = ActorContext.Context;
            
            Vector3 direction = context.transform.position - transform.position;
            direction.Normalize();
            direction.y = 0f;
            eggFighter.rb.linearVelocity = direction * eggFighter.chaseSpeed;
            
            Quaternion rotation = Quaternion.LookRotation(context.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 24f * Time.deltaTime);

            float distance = Vector3.Distance(context.transform.position, transform.position);
            if (distance < 2.5f)
            {
                eggFighter.stateMachine.SetState<EGStatePunch>();
            }
            
            float verticalDiff = context.transform.position.y - transform.position.y;
            bool obstacle = UnityEngine.Physics.Linecast(transform.position, context.transform.position, 1 << LayerMask.NameToLayer("Default"));
            if (distance > eggFighter.chaseMaxDistance || Mathf.Abs(verticalDiff) > 5f || obstacle)
            {
                eggFighter.stateMachine.SetState<EGStateIdle>();
            }
        }
    }
}