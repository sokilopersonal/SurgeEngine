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
        
        private CharacterBase _context;
        
        protected void Launch(CharacterBase context, float pitch)
        {
            if (impulseOnNormal > 0 && impulseOnBoost > 0)
            {
                var boost = context.StateMachine.GetSubState<FBoost>();
                bool boosted = false;
                if (boost != null)
                {
                    boosted = boost.Active;
                }
                
                if (boosted)
                    context.Effects.JumpDeluxEffect.Toggle(true);
                
                _context = context;
                _context.StateMachine.OnStateAssign += DisableCollision;
                
                Physics.IgnoreCollision(GetComponentInChildren<Collider>(), context.Kinematics.Rigidbody.GetComponent<Collider>(), true);
                
                context.transform.forward = Vector3.Cross(-transform.right, Vector3.up);
                context.Kinematics.Rigidbody.linearVelocity = Utility.GetImpulseWithPitch(-transform.forward, transform.right, pitch, !boosted ? impulseOnNormal : impulseOnBoost);

                var jumpPanelState = context.StateMachine.GetState<FStateJumpPanel>();
                jumpPanelState.SetDelux(boosted);
                jumpPanelState.SetKeepVelocity(outOfControl);
                context.StateMachine.SetState<FStateJumpPanel>(true);
                    
                context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
            }
        }

        private void DisableCollision(FState obj)
        {
            if (obj is not FStateJumpPanel)
            {
                Physics.IgnoreCollision(GetComponentInChildren<Collider>(), _context.Kinematics.Rigidbody.GetComponent<Collider>(), false);
                _context.StateMachine.OnStateAssign -= DisableCollision;
            }
        }
    }
}