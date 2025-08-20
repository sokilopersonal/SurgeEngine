using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class Spring : ContactBase
    {
        [SerializeField] protected float speed = 30f;
        [SerializeField] protected float keepVelocityDistance = 5;
        [SerializeField] protected float outOfControl = 0.5f;
        [SerializeField] private bool cancelBoost;
        [SerializeField] private bool isTo3D;
        public float Speed => speed;
        public float KeepVelocityDistance => keepVelocityDistance;

        public override void Contact(Collider msg, CharacterBase context)
        {
            var springState = context.StateMachine.GetState<FStateSpring>();
            if (springState.SpringObject == this) return;
            
            base.Contact(msg, context);
            
            if (cancelBoost) 
                context.StateMachine.GetSubState<FBoost>().Active = false;
            
            springState.SetKeepVelocityDistance(keepVelocityDistance);
            springState.SetSpringObject(this);
            context.StateMachine.SetState<FStateSpring>();
            
            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));

            if (context.Kinematics.Path2D != null)
            {
                if (isTo3D)
                    context.Kinematics.Set2DPath(null);
            }
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, transform.up, Color.green, speed, keepVelocityDistance);
        }
    }
}