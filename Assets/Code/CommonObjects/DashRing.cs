using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SurgeDebug;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class DashRing : ContactBase
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private float keepVelocity;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private float yOffset = 0.5f;

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            var context = ActorContext.Context;
            context.transform.position = transform.position + transform.up * yOffset;

            float dot = Vector3.Dot(transform.up, Vector3.up);
            context.stateMachine.GetSubState<FBoost>().Active = false;
            
            Quaternion rot = Quaternion.LookRotation(transform.up, transform.forward);
            context.transform.rotation = rot;
            context.model.transform.localRotation = rot;
            
            var specialJump = context.stateMachine.SetState<FStateSpecialJump>(0.2f, true, true);
            specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.DashRing, transform.forward, transform.up, dot));
            specialJump.PlaySpecialAnimation(0f);
            specialJump.SetKeepVelocity(keepVelocity);

            Common.ApplyImpulse(transform.forward * speed);
            context.rigidbody.linearVelocity = Vector3.ClampMagnitude(context.rigidbody.linearVelocity, speed);
            context.flags.AddFlag(new Flag(FlagType.OutOfControl, 
                null, true, Mathf.Abs(outOfControl)));
        }

        protected override void Draw()
        {
            base.Draw();
            
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.forward * yOffset, 
                transform.forward, Color.green, speed, keepVelocity);
        }
    }
}