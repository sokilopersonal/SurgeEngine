using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStateChase : EGState
    {
        public EGStateChase(EggFighter eggFighter, Transform transform, Rigidbody rb) : base(eggFighter, transform, rb)
        {
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            var context = ActorContext.Context;
            
            Vector3 direction = context.transform.position - transform.position;
            direction.Normalize();
            direction.y = 0f;
            Rb.linearVelocity = direction * eggFighter.chaseSpeed;
            
            Quaternion rotation = Quaternion.LookRotation(context.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 24f * Time.deltaTime);

            float distance = Vector3.Distance(context.transform.position, transform.position);
            if (distance < 2.5f)
            {
                eggFighter.stateMachine.SetState<EGStatePunch>();
            }
            
            float verticalDiff = context.transform.position.y - transform.position.y;
            bool obstacle = Physics.Linecast(transform.position, context.transform.position, 1 << LayerMask.NameToLayer("Default"));
            if (distance > eggFighter.chaseMaxDistance || Mathf.Abs(verticalDiff) > 5f || obstacle)
            {
                eggFighter.stateMachine.SetState<EGStateIdle>();
            }
        }
    }
}