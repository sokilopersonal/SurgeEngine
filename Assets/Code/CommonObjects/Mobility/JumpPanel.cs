using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.States.SonicSubStates;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.CustomDebug;
using SurgeEngine.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class JumpPanel : ContactBase
    {
        [Header("Properties")]
        [SerializeField, Min(0)] private float impulse = 15f;
        [SerializeField, Range(15, 90)] private float pitch = 10f;
        [SerializeField] private float outOfControl = 0.5f;
        [SerializeField] private bool isDelux;
        [SerializeField] private Transform startPoint;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            ActorBase context = ActorContext.Context;
            if (impulse > 0)
            {
                bool boosted = SonicTools.IsBoost();

                if (boosted)
                    context.effects.jumpDeluxEffect.Toggle(true);

                context.stateMachine.GetSubState<FBoost>().Active = false;

                context.transform.position = startPoint.position ;
                context.transform.forward = Vector3.Cross(-startPoint.right, Vector3.up);
                
                Common.ApplyImpulse(Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, pitch, impulse));
                
                FStateSpecialJump specialJump = context.stateMachine.SetState<FStateSpecialJump>(0.2f, true, true);
                specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.JumpBoard));
                specialJump.PlaySpecialAnimation(0.2f, isDelux || boosted);
                    
                context.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, Mathf.Abs(outOfControl)));
            }
        }

        protected override void OnDrawGizmos()
        {
            TrajectoryDrawer.DrawTrajectory(startPoint.position, Common.GetImpulseWithPitch(Vector3.Cross(-startPoint.right, Vector3.up), startPoint.right, pitch, impulse), Color.green, impulse);
        }
    }
}