using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStateDead : EGState
    {
        private float _timer;
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
        }

        public void ApplyKnockback(Vector3 force, EnemyRagdoll ragdoll)
        {
            ragdoll.Ragdoll(force);
            eggFighter.animation.animator.enabled = false;
        }
    }
}