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
        [SerializeField] protected float keepVelocity;
        [SerializeField] protected float outOfControl = 0.5f;
        [SerializeField] protected float yOffset = 0.5f;
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

            context.StateMachine.GetState<FStateSpecialJump>().SetSpecialData(new SpecialJumpData(SpecialJumpType.Spring, transform, outOfControl)).SetKeepVelocity(keepVelocity);
            context.StateMachine.SetState<FStateSpecialJump>(0f, true, true);

            context.Kinematics.Rigidbody.linearVelocity = transform.up * speed;
            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, 
                null, true, Mathf.Abs(outOfControl)));
        }

        protected override void OnDrawGizmos()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up * yOffset, transform.up, Color.green, speed, keepVelocity);
        }
    }
}