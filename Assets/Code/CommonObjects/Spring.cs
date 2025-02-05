using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Debug;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class Spring : ContactBase
    {
        [SerializeField] protected float speed = 30f;
        [SerializeField] protected float keepVelocity;
        [SerializeField] protected float outOfControl = 0.5f;
        [SerializeField] protected float yOffset = 0.5f;
        [SerializeField] private bool center = true;
        [SerializeField] private bool cancelBoost;

        protected Vector3 direction = Vector3.up;

        protected virtual void Awake()
        {
            direction = transform.up;
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            Actor context = ActorContext.Context;
            
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
            
            FStateSpecialJump specialJump = context.stateMachine.SetState<FStateSpecialJump>(ignoreInactiveDelay: true, allowSameState: true);
            specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.Spring, transform, outOfControl));
            specialJump.PlaySpecialAnimation(0);
            specialJump.SetKeepVelocity(keepVelocity);

            Common.ApplyImpulse(transform.up * speed);
            Rigidbody body = context.kinematics.Rigidbody;
            body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, speed);
            context.flags.AddFlag(new Flag(FlagType.OutOfControl, 
                null, true, Mathf.Abs(outOfControl)));
        }

        protected override void OnDrawGizmos()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up * yOffset, transform.up, Color.green, speed, keepVelocity);
        }
    }
}