using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStateDead : EGState
    {
        private float _timer;
        private bool ragdollCreated = false;

        public EGStateDead(EggFighter eggFighter, Transform transform, Rigidbody rb) : base(eggFighter, transform, rb)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            Actor context = ActorContext.Context;
            if (context.stateMachine.CurrentState is FStateHoming)
            {
                context.stateMachine.SetState<FStateAfterHoming>();
            }

            Object.Destroy(eggFighter.gameObject);
        }

        public void ApplyKnockback(Vector3 force, EnemyRagdoll ragdoll)
        {
            if (ragdollCreated)
                return;

            ragdollCreated = true;

            GameObject newRagdoll = Object.Instantiate(ragdoll.gameObject, transform.parent);
            newRagdoll.transform.position = transform.position;
            newRagdoll.transform.forward = -ActorContext.Context.transform.forward;

            EnemyRagdoll newRagdollScript = newRagdoll.GetComponent<EnemyRagdoll>();

            Object.Destroy(eggFighter.gameObject);

            newRagdollScript.root.AddForce(force * 3f, ForceMode.VelocityChange);
            newRagdollScript.root.AddTorque(force, ForceMode.VelocityChange);
        }
    }
}