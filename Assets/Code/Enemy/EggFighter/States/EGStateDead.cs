using SurgeEngine.Code.ActorStates;
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
            
            var context = ActorContext.Context;
            if (context.stateMachine.CurrentState is FStateHoming)
            {
                context.stateMachine.SetState<FStateAfterHoming>();
            }
            
            eggFighter.view.Destroy();
        }

        public void ApplyKnockback(Vector3 force)
        {
            transform.forward = -ActorContext.Context.transform.forward;
            
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
            Rb.AddForce(force, ForceMode.VelocityChange);
            Rb.AddTorque(force, ForceMode.VelocityChange);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (_timer < 0.25f)
            {
                _timer += dt;
            }
            else
            {
                if (Physics.Raycast(transform.position + Vector3.up, Rb.linearVelocity,
                        2f, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore))
                {
                    Object.Destroy(transform.gameObject);
                }
            }
        }
    }
}