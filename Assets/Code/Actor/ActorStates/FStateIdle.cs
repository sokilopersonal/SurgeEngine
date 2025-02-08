using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateIdle : FStateMove, IDamageableState
    {
        public FStateIdle(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Common.ResetVelocity(ResetVelocityType.Both);

            if (Input.moveVector.magnitude > 0.2f)
            {
                StateMachine.SetState<FStateGround>();
            }

            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.JumpPressed)
                {
                    Kinematics.SetDetachTime(0.2f);
                    StateMachine.SetState<FStateJump>();
                }

                if (Input.BPressed)
                {
                    StateMachine.SetState<FStateSit>();
                }

                if (StateMachine.Exists<FStateQuickstep>())
                {
                    if (Input.LeftBumperPressed)
                    {
                        var qs = StateMachine.GetState<FStateQuickstep>();
                        qs.SetDirection(QuickstepDirection.Left);
                        StateMachine.SetState<FStateQuickstep>();
                    }
                    else if (Input.RightBumperPressed)
                    {
                        var qs = StateMachine.GetState<FStateQuickstep>();
                        qs.SetDirection(QuickstepDirection.Right);
                        StateMachine.SetState<FStateQuickstep>();
                    }
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Common.CheckForGround(out RaycastHit hit))
            {
                Kinematics.Normal = Vector3.up;
                
                Model.RotateBody(Kinematics.Normal);
                Kinematics.Snap(hit.point, Kinematics.Normal, true);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}