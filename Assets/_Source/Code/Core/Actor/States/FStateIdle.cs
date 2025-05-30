using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateIdle : FStateMove, IDamageableState, IBoostHandler
    {
        public FStateIdle(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.ResetVelocity();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Input.moveVector.magnitude > 0.2f || Kinematics.Speed > 0.02f)
            {
                StateMachine.SetState<FStateGround>();
            }

            if (!Actor.Flags.HasFlag(FlagType.OutOfControl))
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
            
            if (Kinematics.CheckForGroundWithDirection(out RaycastHit hit, Vector3.down, 2f))
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;
                
                Vector3 forward = Vector3.Cross(Actor.transform.right, Vector3.up);
                Actor.transform.rotation = Quaternion.LookRotation(forward);
                Model.root.rotation = Quaternion.LookRotation(forward);
                
                Kinematics.Snap(Kinematics.Point, Kinematics.Normal, true);
                Kinematics.SlopePhysics();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public void BoostHandle() { }
    }
}