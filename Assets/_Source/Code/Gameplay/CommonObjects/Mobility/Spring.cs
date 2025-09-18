using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class Spring : StageObject
    {
        [SerializeField] protected float speed = 30f;
        [SerializeField] protected float keepVelocityDistance = 5;
        [SerializeField] protected float outOfControl = 0.5f;
        [SerializeField] protected bool cancelBoost;
        [SerializeField] protected bool isTo3D;
        [SerializeField] protected bool isWallWalk;
        [SerializeField] private EventReference sound;
        public float Speed => speed;
        public float KeepVelocityDistance => keepVelocityDistance;
        public bool IsWallWalk => isWallWalk;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            Launch(context);
        }

        protected virtual void Launch(CharacterBase context)
        {
            var springState = context.StateMachine.GetState<FStateSpring>();
            if (springState.SpringObject == this) return;
            
            if (cancelBoost) 
                context.StateMachine.GetSubState<FBoost>().Active = false;
            
            springState.SetKeepVelocityDistance(keepVelocityDistance);
            springState.SetSpringObject(this);
            context.StateMachine.SetState<FStateSpring>();
            
            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
            
            RuntimeManager.PlayOneShot(sound, transform.position);

            if (context.Kinematics.Path2D != null)
            {
                if (isTo3D)
                {
                    context.Kinematics.Set2DPath(null);
                    context.Kinematics.SetForwardPath(null);
                    context.Kinematics.SetDashPath(null);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, transform.up, Color.green, speed, keepVelocityDistance);
        }
    }
}