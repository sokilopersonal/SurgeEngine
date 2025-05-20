using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.SonicSubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.SonicSpecific
{
    public class FStateSlide : FStateMove, IStateTimeout, IDamageableState
    {
        private readonly SlideConfig _config;
        private readonly QuickStepConfig _quickstepConfig;

        private bool _released;
        
        public FStateSlide(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
            owner.TryGetConfig(out _quickstepConfig);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Timeout = 0.25f;
            StateMachine.GetSubState<FBoost>().Active = false;
            _released = false;
            
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

            if (Input.BReleased)
                _released = true;

            bool ceiling = Common.CheckForCeiling(out RaycastHit data);

            if (Kinematics.Speed < _config.minSpeed || _released)
            {
                if (Kinematics.Speed > _config.minSpeed)
                {
                    if (!ceiling)
                        StateMachine.SetState<FStateGround>();
                }
                else
                {
                    if (!_released)
                    {
                        StateMachine.SetState<FStateSit>();
                    }
                    else
                    {
                        if (!ceiling)
                            StateMachine.SetState<FStateIdle>();
                        else
                            StateMachine.SetState<FStateSit>();
                    }
                }
            }

            if (_quickstepConfig && !ceiling)
            {
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
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Common.CheckForGround(out RaycastHit hit))
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = hit.normal;
                Kinematics.Snap(hit.point, hit.normal, true);

                Kinematics.WriteMovementVector(_rigidbody.linearVelocity);
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, Vector3.zero, _config.deceleration * dt);
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, hit.normal);
                Model.RotateBody(hit.normal);
                Kinematics.SlopePhysics();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }

            HurtBox.Create(Actor, 
                Actor.transform.position + new Vector3(0f, 0.25f, 0.25f),
                Actor.transform.rotation, new Vector3(0.5f, 0.5f, 0.75f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
        }
        
        public float Timeout { get; set; }
    }
}