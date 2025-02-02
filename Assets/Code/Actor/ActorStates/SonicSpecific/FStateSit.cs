using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateSit : FStateMove, ISweepKickHandler
    {
        public FStateSit(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Common.ResetVelocity(ResetVelocityType.Both);
            Model.SetLowerCollision();
        }

        public override void OnExit()
        {
            base.OnExit();
            Model.ResetCollisionToDefault();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Input.BHeld)
            {
                StateMachine.SetState<FStateIdle>();
            }

            if (Input.JumpPressed)
            {
                StateMachine.SetState<FStateJump>(0.1f);
            }

            if (Input.moveVector.magnitude >= 0.5f)
            {
                StateMachine.SetState<FStateCrawl>();
            }

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

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Common.CheckForGround(out RaycastHit hit))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;

                Kinematics.Snap(point, normal, true);
            }
        }
    }
}