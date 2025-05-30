using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class DashRing : ContactBase
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private float keepVelocity;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private float yOffset = 0.5f;
        [SerializeField] private bool center = true;
        [SerializeField] private bool cancelBoost;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            if (center)
            {
                Vector3 target = transform.position + transform.up * yOffset;
                context.PutIn(target);
            }
            else
            {
                Vector3 target = transform.up * yOffset;
                context.AddIn(target);
            }
            if (cancelBoost) context.StateMachine.GetSubState<FBoost>().Active = false;
            
            FStateSpecialJump specialJump = context.StateMachine.SetState<FStateSpecialJump>(0.2f, true, true);
            specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.DashRing, transform, outOfControl));
            specialJump.PlaySpecialAnimation(0f);
            specialJump.SetKeepVelocity(keepVelocity);

            context.Kinematics.Rigidbody.linearVelocity = transform.up * speed;
            Rigidbody body = context.Kinematics.Rigidbody;
            body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, speed);
            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, 
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