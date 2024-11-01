using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStateChase : EGState
    {
        public EGStateChase(EggFighter eggFighter, Transform transform, NavMeshAgent agent) : base(eggFighter, transform, agent)
        {
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            var context = ActorContext.Context;
            agent.SetDestination(context.transform.position);
            
            Quaternion rotation = Quaternion.LookRotation(context.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 12f * Time.deltaTime);

            if (Vector3.Distance(context.transform.position, transform.position) < 2.2f)
            {
                agent.isStopped = true;
                eggFighter.stateMachine.SetState<EGStatePunch>();
            }
        }
    }
}