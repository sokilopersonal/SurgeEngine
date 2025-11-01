using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public abstract class JumpPanelBase : StageObject
    {
        [SerializeField] protected float impulseOnNormal = 30;
        [SerializeField] protected float impulseOnBoost = 30;
        [SerializeField] protected float outOfControl = 0.5f;
        [SerializeField] private Collider collision;
        
        protected void Launch(CharacterBase context, float pitch)
        {
            bool boosted = false;
            if (context.StateMachine.GetState(out FBoost boost))
            {
                if (boost != null)
                {
                    boosted = boost.Active;
                }
            }

            if (boosted)
                context.Effects.JumpDeluxEffect.Toggle(true);

            var disableCollision = new DisableCollision();

            context.transform.forward = Vector3.Cross(-transform.right, Vector3.up);
            context.Kinematics.Rigidbody.linearVelocity = Utility.GetImpulseWithPitch(-transform.forward, transform.right, pitch, !boosted ? impulseOnNormal : impulseOnBoost);
            
            disableCollision.Disable(context, collision, typeof(FStateJumpPanel));

            var jumpPanelState = context.StateMachine.GetState<FStateJumpPanel>();
            jumpPanelState.SetDelux(boosted);
            jumpPanelState.SetKeepVelocity(outOfControl);
            context.StateMachine.SetState<FStateJumpPanel>(true);
                    
            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
        }
    }
}