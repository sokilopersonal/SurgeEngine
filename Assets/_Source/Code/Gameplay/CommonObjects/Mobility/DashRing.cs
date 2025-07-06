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
        [SerializeField] private float keepVelocityDistance = 5f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private bool cancelBoost;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);

            if (cancelBoost) context.StateMachine.GetSubState<FBoost>().Active = false;
            
            context.StateMachine.GetState<FStateDashRing>().SetKeepVelocityDistance(keepVelocityDistance);
            context.StateMachine.SetState<FStateDashRing>(0f, true, true);

            Rigidbody body = context.Kinematics.Rigidbody;
            body.position = transform.position;
            body.linearVelocity = transform.up * speed;
            body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, speed);
            
            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, 
                null, true, Mathf.Abs(outOfControl)));
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up, 
                transform.up, Color.green, speed, keepVelocityDistance);
        }
    }
}