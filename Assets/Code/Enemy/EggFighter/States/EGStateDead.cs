using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters;
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
            
            Object.Destroy(transform.gameObject, 2f);

            _timer = 0;

            var collider = eggFighter.view.eCollider;
            collider.radius = 0.75f;
            collider.height = 1.5f;
            
            Rb.freezeRotation = false;
            
            var context = ActorContext.Context;
            if (context.stateMachine.CurrentState is FStateHoming)
            {
                context.stateMachine.SetState<FStateAfterHoming>();
            }
        }

        public void ApplyKnockback(Vector3 force)
        {
            transform.forward = -ActorContext.Context.transform.forward;
            
            Rb.linearVelocity = Vector3.zero;
            Rb.AddForce(force, ForceMode.VelocityChange);
            Rb.angularVelocity = Vector3.zero;
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
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, 
                        0.85f, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore))
                {
                    Object.Destroy(transform.gameObject);
                }
            }
        }
    }
}