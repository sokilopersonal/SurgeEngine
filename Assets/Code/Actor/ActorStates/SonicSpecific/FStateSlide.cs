using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateSlide : FStateMove, IStateTimeout
    {
        private readonly SlideConfig _config;
        private readonly QuickStepConfig _quickstepConfig;
        
        public FStateSlide(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _config = (owner as Sonic).slideConfig;
            _quickstepConfig = (owner as Sonic).quickstepConfig;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Timeout = 0.3f;
            StateMachine.GetSubState<FBoost>().Active = false;
            
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

            if (Stats.currentSpeed < _config.minSpeed || !Input.BHeld)
            {
                if (Stats.currentSpeed > _config.minSpeed)
                {
                    StateMachine.SetState<FStateGround>();
                }
                else
                {
                    if (Input.BHeld)
                        StateMachine.SetState<FStateSit>();
                    else
                        StateMachine.SetState<FStateIdle>();
                }
            }

            if (Input.LeftBumperPressed)
            {
                if (Kinematics.HorizontalSpeed >= _quickstepConfig.minSpeed)
                {
                    var qs = StateMachine.GetState<FStateRunQuickstep>();
                    qs.SetDirection(QuickstepDirection.Left);
                    StateMachine.SetState<FStateRunQuickstep>();
                }
                else
                {
                    var qs = StateMachine.GetState<FStateQuickstep>();
                    qs.SetDirection(QuickstepDirection.Left);
                    StateMachine.SetState<FStateQuickstep>();
                }
            }
            else if (Input.RightBumperPressed)
            {
                if (Kinematics.HorizontalSpeed >= _quickstepConfig.minSpeed)
                {
                    var qs = StateMachine.GetState<FStateRunQuickstep>();
                    qs.SetDirection(QuickstepDirection.Right);
                    StateMachine.SetState<FStateRunQuickstep>();
                }
                else
                {
                    var qs = StateMachine.GetState<FStateQuickstep>();
                    qs.SetDirection(QuickstepDirection.Right);
                    StateMachine.SetState<FStateQuickstep>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Common.CheckForGround(out RaycastHit hit, CheckGroundType.Normal, 5f))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;
                Kinematics.Normal = normal;

                Kinematics.WriteMovementVector(_rigidbody.linearVelocity);
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, Vector3.zero, _config.deceleration * dt);
                Model.RotateBody(normal);
                Kinematics.Snap(point, normal, true);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }

            HurtBox.Create(Actor, Actor.transform.position + new Vector3(0f, 0.25f, 0.25f), Actor.transform.rotation, new Vector3(0.5f, 0.5f, 0.75f));
        }
        
        public SlideConfig GetConfig() => _config;
        public float Timeout { get; set; }
    }
}