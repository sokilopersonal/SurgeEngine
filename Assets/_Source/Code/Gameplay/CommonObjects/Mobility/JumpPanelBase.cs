using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public abstract class JumpPanelBase : ContactBase
    {
        [SerializeField] protected float impulse = 30;
        [SerializeField] protected float outOfControl = 0.5f;
        
        protected void Launch(ActorBase context, float pitch)
        {
            if (impulse > 0)
            {
                bool boosted = SonicTools.IsBoost();
                if (boosted)
                    context.Effects.JumpDeluxEffect.Toggle(true);
                
                context.transform.forward = Vector3.Cross(-transform.right, Vector3.up);
                context.Kinematics.Rigidbody.linearVelocity = Utility.GetImpulseWithPitch(-transform.forward, transform.right, pitch, impulse);

                var jumpPanelState = context.StateMachine.GetState<FStateJumpPanel>();
                jumpPanelState.SetDelux(boosted);
                jumpPanelState.SetKeepVelocity(outOfControl);
                context.StateMachine.SetState<FStateJumpPanel>(true);
                    
                context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
            }
        }
    }
}