using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States
{
    public class FStateIdle : FStateMove, IDamageableState
    {
        public FStateIdle(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
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
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;
                
                Model.RotateBody(Kinematics.Normal);
                Kinematics.Snap(Kinematics.Point, Kinematics.Normal, true);
                
                Kinematics.SlopePhysics();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}