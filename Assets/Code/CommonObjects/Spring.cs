using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SurgeDebug;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class Spring : ContactBase
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private float keepVelocity;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private float yOffset = 0.5f;
        
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
            context.transform.position = transform.position + direction * yOffset;

            float dot = Vector3.Dot(transform.up, Vector3.up);
            context.stateMachine.SetState<FStateAir>();
            context.stateMachine.GetSubState<FBoost>().Active = false;
            
            context.transform.rotation = Quaternion.LookRotation(transform.forward, direction);
            context.model.transform.localRotation = Quaternion.LookRotation(transform.forward, direction);
            
            var specialJump = context.stateMachine.SetState<FStateSpecialJump>(0.2f, true, true);
            specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.Spring, transform.forward, direction, dot));
            specialJump.PlaySpecialAnimation(0);
            specialJump.SetKeepVelocity(keepVelocity);

            Common.ApplyImpulse(transform.up * speed);
            context.rigidbody.linearVelocity = Vector3.ClampMagnitude(context.rigidbody.linearVelocity, speed);
            context.flags.AddFlag(new Flag(FlagType.OutOfControl, 
                null, true, Mathf.Abs(outOfControl)));
        }

        protected override void Draw()
        {
            base.Draw();
            
            Vector3 dir = GetType() == typeof(Spring) ? transform.up : Vector3.up;
            
            TrajectoryDrawer.DrawTrajectory(transform.position + dir * yOffset, 
                dir, Color.green, speed, keepVelocity);
        }
    }
}