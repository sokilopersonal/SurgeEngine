using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class DashPanel : ContactBase
    {
        [SerializeField] private float speed = 35f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private bool center;

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            var context = ActorContext.Context;
            
            context.stateMachine.GetSubState<FBoost>().Active = false;
            
            Common.ApplyImpulse(transform.forward * speed);
            context.stateMachine.SetState<FStateGround>();

            if (center)
            {
                context.transform.position = transform.position;
                context.transform.forward = Vector3.Cross(-transform.right, Vector3.up);
            }

            context.flags.AddFlag(new Flag(FlagType.OutOfControl, 
                new [] { Tags.AllowBoost }, true, Mathf.Abs(outOfControl)));
        }
    }
}