using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.SurgeDebug;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class Spring : ContactBase
    {
        [SerializeField] protected float speed = 30f;
        [SerializeField] protected float keepVelocity;
        [SerializeField] protected float outOfControl = 0.5f;
        [SerializeField] protected float yOffset = 0.5f;

        protected Vector3 direction = Vector3.up;

        protected virtual void Awake()
        {
            direction = transform.up;
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            var context = ActorContext.Context;
            context.kinematics.Rigidbody.position = transform.position + direction * (yOffset * 2);
            context.stateMachine.SetState<FStateAir>();
            context.stateMachine.GetSubState<FBoost>().Active = false;
            
            var specialJump = context.stateMachine.SetState<FStateSpecialJump>(allowSameState: true);
            specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.Spring, transform));
            specialJump.PlaySpecialAnimation(0);
            specialJump.SetKeepVelocity(keepVelocity);

            Common.ApplyImpulse(transform.up * speed);
            var body = context.kinematics.Rigidbody;
            body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, speed);
            context.flags.AddFlag(new Flag(FlagType.OutOfControl, 
                null, true, Mathf.Abs(outOfControl)));
        }

        protected override void Draw()
        {
            base.Draw();
            
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up * yOffset, transform.up, Color.green, speed, keepVelocity);
        }
    }
}