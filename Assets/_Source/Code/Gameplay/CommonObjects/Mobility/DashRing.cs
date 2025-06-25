using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class DashRing : ContactBase
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private float keepVelocity;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private bool cancelBoost;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            if (cancelBoost) context.StateMachine.GetSubState<FBoost>().Active = false;
            
            context.StateMachine.GetState<FStateSpecialJump>().SetSpecialData(new SpecialJumpData(SpecialJumpType.DashRing, transform, outOfControl)).SetKeepVelocity(keepVelocity);
            context.StateMachine.SetState<FStateSpecialJump>(0f, true, true);

            context.Kinematics.Rigidbody.linearVelocity = transform.up * speed;
            Rigidbody body = context.Kinematics.Rigidbody;
            body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, speed);
            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, 
                null, true, Mathf.Abs(outOfControl)));
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up, 
                transform.up, Color.green, speed, keepVelocity);
        }
    }
}