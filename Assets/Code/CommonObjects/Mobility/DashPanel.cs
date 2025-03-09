using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Misc;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace SurgeEngine.Code.CommonObjects
{
    public class DashPanel : ContactBase
    {
        [SerializeField] private float speed = 35f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private bool center;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            ActorBase context = ActorContext.Context;
            Rigidbody body = context.kinematics.Rigidbody;
            
            if (center)
            {
                body.linearVelocity = Vector3.zero;
                body.position = transform.position + transform.up * 0.5f;
            }
            
            context.animation.StateAnimator.TransitionToState(AnimatorParams.RunCycle);
            body.linearVelocity = transform.forward * speed;
            context.stateMachine.SetState<FStateGround>();
            
            body.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            context.model.root.rotation = body.rotation;

            context.flags.AddFlag(new Flag(FlagType.OutOfControl, 
                new [] { Tags.AllowBoost }, true, Mathf.Abs(outOfControl)));
            
            new Rumble().Vibrate(0.7f, 0.9f, 0.5f);
        }

        protected override void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, transform.forward * speed * outOfControl, Color.green);
        }
    }
}