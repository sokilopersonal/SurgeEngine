using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class JumpPanel : ContactBase
    {
        [Header("Properties")]
        [SerializeField, Min(0)] private float impulse = 15f;
        [SerializeField] private float outOfControl = 0.5f;
        
        private Vector3 StartPosition => transform.position + Vector3.up;
        private float Pitch => 38f;
        
        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            if (impulse > 0)
            {
                bool boosted = SonicTools.IsBoost();
                if (boosted)
                    context.Effects.JumpDeluxEffect.Toggle(true);

                context.StateMachine.GetSubState<FBoost>().Active = false;
                
                context.transform.forward = Vector3.Cross(-transform.right, Vector3.up);
                context.Kinematics.Rigidbody.linearVelocity = Utility.GetImpulseWithPitch(-transform.forward, transform.right, Pitch, impulse);

                var jumpPanelState = context.StateMachine.GetState<FStateJumpPanel>();
                jumpPanelState.SetDelux(boosted);
                jumpPanelState.SetKeepVelocity(outOfControl);
                context.StateMachine.SetState<FStateJumpPanel>(0f, true, true);
                    
                context.Flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, Mathf.Abs(outOfControl)));
            }
        }

        protected override void OnDrawGizmos()
        {
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, Pitch, impulse), Color.green, impulse);
        }
    }
}