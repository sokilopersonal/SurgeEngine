using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class DashPanel : ContactBase
    {
        [SerializeField] private float speed = 35f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private bool center;

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            var context = ActorContext.Context;
            
            context.rigidbody.linearVelocity = Quaternion.FromToRotation(transform.forward, transform.up) * transform.forward * speed; // Rotate velocity to prevent high speed/boost issues
            context.rigidbody.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            context.animation.TransitionToState(AnimatorParams.RunCycle, 0f);
            if (context.stats.currentSpeed < speed)
            {
                Common.ApplyImpulse(transform.forward * speed);
            }
            context.stateMachine.SetState<FStateGround>();

            if (center)
            {
                context.rigidbody.position = transform.position;
            }

            context.flags.AddFlag(new Flag(FlagType.OutOfControl, 
                new [] { Tags.AllowBoost }, true, Mathf.Abs(outOfControl)));
        }

        protected override void Draw()
        {
            base.Draw();    
            
            Debug.DrawRay(transform.position, transform.forward * speed * outOfControl, Color.green);
        }
    }
}