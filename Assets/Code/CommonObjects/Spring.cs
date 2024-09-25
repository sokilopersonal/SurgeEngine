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

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            var context = ActorContext.Context;
            context.transform.position = transform.position + transform.up * yOffset;

            float dot = Vector3.Dot(transform.up, Vector3.up);
            context.stateMachine.SetState<FStateAir>();
            context.stateMachine.GetSubState<FBoost>().Active = false;
            
            context.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            context.model.transform.localRotation = Quaternion.LookRotation(transform.forward, transform.up);
            
            var specialJump = context.stateMachine.CurrentState is FStateSpecialJump ? 
                context.stateMachine.GetState<FStateSpecialJump>() : context.stateMachine.SetState<FStateSpecialJump>();
            specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.Spring, transform.forward, transform.up, dot));
            specialJump.PlaySpecialAnimation(0.2f);
            specialJump.SetKeepVelocity(keepVelocity);

            Common.ApplyImpulse(transform.up * speed);
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