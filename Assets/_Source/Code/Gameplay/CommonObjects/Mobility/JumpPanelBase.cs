using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using SurgeEngine._Source.Code.Infrastructure.Tools;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public abstract class JumpPanelBase : StageObject
    {
        [SerializeField] protected float impulse = 30;
        [SerializeField] protected float outOfControl = 0.5f;
        
        protected void Launch(CharacterBase context, float pitch)
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