using System;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Core.Actor.States.SonicSubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.Inputs;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.Core.Actor.States
{
    public sealed class FStateGround : FStateMove, IBoostHandler, IDamageableState
    {
        private GroundTag _surfaceTag;
        
        public event Action<GroundTag> OnSurfaceTagChanged;
        
        private readonly QuickStepConfig _quickstepConfig;
        private readonly SlideConfig _slideConfig;

        public FStateGround(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _quickstepConfig);
            owner.TryGetConfig(out _slideConfig);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.SetDetachTime(0f);
            ConvertAirToGroundVelocity();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.JumpPressed)
                {
                    StateMachine.SetState<FStateJump>(0.1f);
                }
                
                if (!SonicTools.IsBoost())
                {
                    if (Kinematics.Skidding && Kinematics.HorizontalSpeed > 15f)
                    {
                        StateMachine.SetState<FStateBrake>();
                    }
                }

                float minSpeed = _slideConfig.minSpeed;
                minSpeed += minSpeed * 1.5f;
                float dot = Stats.moveDot;
                float abs = Mathf.Abs(dot);
            
                bool readyForDrift = Kinematics.Speed > 5f && abs < 0.4f && !Mathf.Approximately(dot, 0f);
                bool readyForSlide = Kinematics.Speed > minSpeed;

                if (_quickstepConfig && StateMachine.Exists<FStateRunQuickstep>())
                {
                    if (Input.LeftBumperPressed)
                    {
                        if (Kinematics.Speed >= _quickstepConfig.minSpeed)
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
                        if (Kinematics.Speed >= _quickstepConfig.minSpeed)
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
                
                if (Input.BHeld)
                {
                    if (readyForSlide && !readyForDrift)
                    {
                        StateMachine.SetState<FStateSlide>();
                    }
                    else if(!readyForSlide && !readyForDrift)
                    {
                        StateMachine.SetState<FStateCrawl>();
                    }

                    if (readyForDrift)
                    {
                        StateMachine.SetState<FStateDrift>();
                    }
                }

                if (SonicInputLayout.DriftHeld)
                {
                    if (readyForDrift)
                    {
                        StateMachine.SetState<FStateDrift>();
                    }
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Vector3 prevNormal = Kinematics.Normal;
            BaseActorConfig config = Actor.config;
            float distance = config.castDistance * config.castDistanceCurve
                .Evaluate(Kinematics.HorizontalSpeed / config.topSpeed);
            bool checkForPredictedGround =
                Kinematics.CheckForPredictedGround(Kinematics.Velocity, Kinematics.Normal, Time.fixedDeltaTime, distance, 8);
            if (Kinematics.CheckForGround(out RaycastHit data, castDistance: distance) && checkForPredictedGround)
            {
                Kinematics.Point = data.point;
                Kinematics.SlerpSnapNormal(data.normal);
                
                Vector3 stored = Vector3.ClampMagnitude(_rigidbody.linearVelocity, config.maxSpeed);
                _rigidbody.linearVelocity = Quaternion.FromToRotation(_rigidbody.transform.up, prevNormal) * stored;

                Kinematics.BasePhysics(Kinematics.Normal);
                Kinematics.Snap(Kinematics.Point, Kinematics.Normal, true);
                Model.RotateBody(Kinematics.Normal);
                Kinematics.SlopePhysics();
                
                UpdateSurfaceTag(data.transform.gameObject.GetGroundTag());
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }

            if (Kinematics.CheckForGroundWithDirection(out RaycastHit verticalHit, _rigidbody.transform.up) && Kinematics.Angle >= 90)
            {
                Kinematics.Point = verticalHit.point;
                Kinematics.Normal = verticalHit.normal;
            }
        }
        
        private void UpdateSurfaceTag(GroundTag newTag)
        {
            if (_surfaceTag != newTag)
            {
                _surfaceTag = newTag;
                OnSurfaceTagChanged?.Invoke(newTag);
            }
        }

        public void BoostHandle()
        {
            Actor.stateMachine.GetSubState<FBoost>().BaseGroundBoost();
        }

        private void ConvertAirToGroundVelocity()
        {
            if (Physics.Raycast(Actor.transform.position, _rigidbody.linearVelocity.normalized, out RaycastHit velocityFix, _rigidbody.linearVelocity.magnitude, Actor.config.castLayer))
            {
                float nextGroundAngle = Vector3.Angle(velocityFix.normal, Vector3.up);
                if (nextGroundAngle <= Kinematics.maxAngleDifference)
                {
                    Vector3 fixedVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, Kinematics.Normal);
                    fixedVelocity = Quaternion.FromToRotation(Actor.transform.up, velocityFix.normal) * fixedVelocity;
                    _rigidbody.linearVelocity = fixedVelocity;
                }
            }
        }

        public GroundTag GetSurfaceTag() => _surfaceTag;
    }

    public enum GroundTag
    {
        Grass,
        Concrete,
        Water
    }
}