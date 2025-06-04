using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.Inputs;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class DashPanel : ContactBase
    {
        [SerializeField] private float speed = 35f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private bool center;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            Rigidbody body = context.Kinematics.Rigidbody;
            
            if (center)
            {
                Vector3 target = transform.position + transform.up * 0.25f;
                context.Camera.StateMachine.SetLateOffset(context.transform.position - target - transform.up * 0.75f); // We need to use this and not just context.PutIn because the late offset is not calculated properly
                context.transform.position = target;
            }

            var bodySpeed = context.Kinematics.Speed;
            if (bodySpeed < speed)
            {
                body.linearVelocity = transform.forward * speed;
            }
            else
            {
                body.linearVelocity = transform.forward * bodySpeed;
            }
            
            context.StateMachine.SetState<FStateGround>();
            
            body.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            context.Model.root.rotation = body.rotation;

            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, 
                new [] { Tags.AllowBoost }, true, Mathf.Abs(outOfControl)));
            
            new Rumble().Vibrate(0.7f, 0.9f, 0.5f);
        }

        protected override void OnDrawGizmos()
        {
            Debug.DrawRay(transform.position, transform.forward * speed * outOfControl, Color.green);
        }
    }
}