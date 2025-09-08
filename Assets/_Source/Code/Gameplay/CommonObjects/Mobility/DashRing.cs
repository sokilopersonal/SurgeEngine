using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class DashRing : StageObject
    {
        [SerializeField] protected float speed = 30f;
        [SerializeField] protected float keepVelocityDistance = 5f;
        [SerializeField] protected float outOfControl = 0.5f;
        [SerializeField] protected bool cancelBoost;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            if (cancelBoost && context.StateMachine.GetState(out FBoost boost))
            {
                boost.Active = false;
            }
            
            context.StateMachine.GetState<FStateDashRing>().SetKeepVelocityDistance(keepVelocityDistance);
            context.StateMachine.SetState<FStateDashRing>(true);

            Rigidbody body = context.Kinematics.Rigidbody;
            body.position = transform.position;
            body.linearVelocity = -transform.forward * speed;
            
            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, 
                -transform.forward, Color.green, speed, keepVelocityDistance);
        }
    }
}