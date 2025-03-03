using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateSit : FStateMove
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

            if (Input.moveVector.magnitude >= 0.2f)
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

            Vector3 prevNormal = Kinematics.Normal;
            BaseActorConfig config = Actor.config;
            float distance = config.castDistance;
            if (Common.CheckForGround(out RaycastHit data, castDistance: distance))
            {
                Kinematics.Point = data.point;
                Kinematics.SlerpSnapNormal(data.normal);

                Vector3 stored = Vector3.ClampMagnitude(_rigidbody.linearVelocity, config.maxSpeed);
                _rigidbody.linearVelocity = Quaternion.FromToRotation(_rigidbody.transform.up, prevNormal) * stored;

                Actor.kinematics.BasePhysics(Kinematics.Point, Kinematics.Normal);
                Model.RotateBody(Kinematics.Normal);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}