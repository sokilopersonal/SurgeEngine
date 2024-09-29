using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SurgeDebug;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class JumpPanel : ContactBase
    {
        [Header("Properties")]
        [SerializeField, Min(0)] private float impulse = 15f;
        [SerializeField, Range(15, 90)] private float pitch = 10f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private Transform startPoint;

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);

            var context = ActorContext.Context;
            Vector3 cross = Common.GetCross(transform, pitch, true);
            if (impulse > 0)
            {
                context.stateMachine.GetSubState<FBoost>().Active = false;

                context.transform.position = startPoint.position ;
                context.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                
                Common.ApplyImpulse(Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, pitch, impulse));
                
                var specialJump = context.stateMachine.CurrentState is FStateSpecialJump ? 
                    context.stateMachine.GetState<FStateSpecialJump>() : context.stateMachine.SetState<FStateSpecialJump>(0.1f);
                specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.JumpBoard));
                specialJump.PlaySpecialAnimation(0.2f);
                    
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, Mathf.Abs(outOfControl)));
            }
        }

        protected override void Draw()
        {
            base.Draw();
            
            TrajectoryDrawer.DrawTrajectory(startPoint.position, Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, pitch, impulse), Color.green, impulse);
        }
    }
}