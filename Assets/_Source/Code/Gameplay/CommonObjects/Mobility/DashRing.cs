using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class DashRing : ContactBase
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private float keepVelocityDistance = 5f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private bool cancelBoost;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            if (cancelBoost) context.StateMachine.GetSubState<FBoost>().Active = false;
            
            context.StateMachine.GetState<FStateDashRing>().SetKeepVelocityDistance(keepVelocityDistance);
            context.StateMachine.SetState<FStateDashRing>(true);

            Rigidbody body = context.Kinematics.Rigidbody;
            body.position = transform.position;
            body.linearVelocity = transform.up * speed;
            body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, speed);
            
            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, 
                transform.up, Color.green, speed, keepVelocityDistance);
        }
    }
}