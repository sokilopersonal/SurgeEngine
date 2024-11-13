
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Misc;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

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
            context.rigidbody.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            context.animation.TransitionToState(AnimatorParams.RunCycle, 0f);
            Common.ApplyImpulse(transform.forward * speed);
            context.stateMachine.SetState<FStateGround>(outOfControl);

            if (center)
            {
                context.rigidbody.position = transform.position + transform.up * 0.5f;
            }

            context.flags.AddFlag(new Flag(FlagType.OutOfControl, 
                new [] { Tags.AllowBoost }, true, Mathf.Abs(outOfControl)));
            
            new Rumble().Vibrate(0.7f, 0.9f, 0.5f);
        }

        protected override void Draw()
        {
            base.Draw();    
            
            Debug.DrawRay(transform.position, transform.forward * speed * outOfControl, Color.green);
        }
    }
}