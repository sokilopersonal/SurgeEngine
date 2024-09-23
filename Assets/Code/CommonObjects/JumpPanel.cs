using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SurgeDebug;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class JumpPanel : ActorCollision
    {
        [Header("Properties")]
        [SerializeField, Min(0)] private float impulse = 15f;
        [SerializeField, Range(0, 75)] private float pitch = 10f;
        [SerializeField] private float outOfControl = 0.5f;

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            var context = ActorContext.Context;
            //float impulse = context.stateMachine.GetSubState<FBoost>().Active ? impulseOnBoost : impulseOnNormal;
            Vector3 cross = Common.GetCross(transform, pitch, true);
            if (impulse > 0)
            {
                context.stateMachine.GetSubState<FBoost>().Active = false;
                
                context.rigidbody.linearVelocity = Vector3.zero;
                context.stats.planarVelocity = Vector3.zero;
                context.stats.movementVector = Vector3.zero;

                context.transform.position = transform.position + Vector3.up * 1.2f;
                context.transform.forward = cross;
                
                context.stateMachine.SetState<FStateAir>();
                context.animation.TransitionToState("Jump Delux", 0.2f, true);

                Vector3 impulseV = cross.normalized * impulse;
                        
                context.AddImpulse(impulseV);
                context.rigidbody.linearVelocity = Vector3.ClampMagnitude(context.rigidbody.linearVelocity, impulse);
                    
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
            }
        }

        protected override void Draw()
        {
            base.Draw();
            
            TrajectoryDrawer.DrawTrajectory(transform.position + Vector3.up * 0.75f, Common.GetCross(transform, pitch, true), impulse, Color.green);
        }
    }
}