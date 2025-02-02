using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Inputs;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.Tools;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.ActorStates
{
    public sealed class FStateGround : FStateMove, IBoostHandler, ISweepKickHandler
    {
        private string _surfaceTag;
        private readonly QuickStepConfig _quickstepConfig;
        
        public FStateGround(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _quickstepConfig = (owner as Sonic).quickstepConfig;
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

                float minSpeed = StateMachine.GetState<FStateSlide>().GetConfig().minSpeed;
                minSpeed += minSpeed * 1.5f;
                float dot = Stats.moveDot;
                float abs = Mathf.Abs(dot);
            
                bool readyForDrift = Kinematics.HorizontalSpeed > 5f && abs < 0.4f && !Mathf.Approximately(dot, 0f);
                bool readyForSlide = Kinematics.HorizontalSpeed > minSpeed;

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
            if (Common.CheckForGround(out RaycastHit data, castDistance: distance) && 
                Kinematics.CheckForPredictedGround(_rigidbody.linearVelocity, Kinematics.Normal, Time.fixedDeltaTime, distance, 6))
            {
                Kinematics.Point = data.point;
                Kinematics.Normal = data.normal;
                
                Vector3 stored = Vector3.ClampMagnitude(_rigidbody.linearVelocity, config.maxSpeed);
                _rigidbody.linearVelocity = Quaternion.FromToRotation(_rigidbody.transform.up, prevNormal) * stored;

                Actor.kinematics.BasePhysics(Kinematics.Point, Kinematics.Normal);
                Model.RotateBody(Kinematics.Normal);
                
                _surfaceTag = data.transform.gameObject.GetGroundTag();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
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

        public string GetSurfaceTag() => _surfaceTag;
    }
}