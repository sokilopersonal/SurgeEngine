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

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            ApplyImpulse(direction);
        }

        private void ApplyImpulse(Vector3 direction)
        {
            var context = ActorContext.Context;
            context.transform.position = transform.position + direction * (yOffset * 2);

            float dot = Vector3.Dot(transform.up, Vector3.up);
            context.stateMachine.SetState<FStateAir>();
            context.stateMachine.GetSubState<FBoost>().Active = false;
            
            context.transform.rotation = Quaternion.LookRotation(transform.forward, direction);
            context.model.transform.localRotation = Quaternion.LookRotation(transform.forward, direction);
            
            var specialJump = context.stateMachine.SetState<FStateSpecialJump>(0.2f, true, true);
            specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.Spring));
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
            
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up * yOffset, 
                transform.up, Color.green, speed, keepVelocity);
        }
    }
}