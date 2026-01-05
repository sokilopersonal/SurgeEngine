using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class Spring : StageObject
    {
        [SerializeField] protected float speed = 30f;
        [SerializeField] protected float keepVelocityDistance = 5;
        [SerializeField] protected float outOfControl = 0.5f;
        [SerializeField] protected bool cancelBoost;
        [SerializeField] protected bool isTo3D;
        [SerializeField] protected bool isWallWalk;
        [SerializeField] private bool hasBase;
        [SerializeField] private EventReference sound;
        [SerializeField] GameObject baseModel;
        public float Speed => speed;
        public virtual float KeepVelocityDistance => keepVelocityDistance;
        public bool IsWallWalk => isWallWalk;
        public virtual Vector3 Direction => transform.up;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            Launch(context);
        }

        protected virtual void Launch(CharacterBase context)
        {
            var springState = context.StateMachine.GetState<FStateSpring>();
            if (springState.SpringObject == this) return;
            
            if (cancelBoost && context.StateMachine.GetState(out FBoost boost)) 
                boost.Active = false;
            
            springState.SetKeepVelocityDistance(keepVelocityDistance);
            springState.SetSpringObject(this);
            context.StateMachine.SetState<FStateSpring>();
            Utility.MoveToPosition(this, context.Kinematics.Rigidbody, transform.position, Direction * speed, 0.1f);
            
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

        private void OnValidate()
        {
            if (baseModel)
                baseModel.SetActive(hasBase);
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, Direction, Color.green, speed, keepVelocityDistance);
        }
    }
}