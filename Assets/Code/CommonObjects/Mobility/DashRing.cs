using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.States.SonicSubStates;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.CustomDebug;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class DashRing : ContactBase
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private float keepVelocity;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private float yOffset = 0.5f;
        [SerializeField] private bool center = true;
        [SerializeField] private bool cancelBoost;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            ActorBase context = ActorContext.Context;

            if (center)
            {
                Vector3 target = transform.position + transform.up * yOffset;
                context.transform.position = target;
            }
            else
            {
                Vector3 target = transform.up * yOffset;
                context.transform.position += target;
            }
            if (cancelBoost) context.stateMachine.GetSubState<FBoost>().Active = false;
            
            FStateSpecialJump specialJump = context.stateMachine.SetState<FStateSpecialJump>(0.2f, true, true);
            specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.DashRing, transform, outOfControl));
            specialJump.PlaySpecialAnimation(0f);
            specialJump.SetKeepVelocity(keepVelocity);

            Common.ApplyImpulse(transform.up * speed);
            Rigidbody body = context.kinematics.Rigidbody;
            body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, speed);
            context.flags.AddFlag(new Flag(FlagType.OutOfControl, 
                null, true, Mathf.Abs(outOfControl)));
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up * yOffset, 
                transform.up, Color.green, speed, keepVelocity);
        }
    }
}