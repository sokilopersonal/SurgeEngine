using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateCrawl : FStateMove
    {
        private string _surfaceTag;

        private readonly CrawlConfig _crawlConfig;
        private readonly QuickStepConfig _quickstepConfig;
        
        public FStateCrawl(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _crawlConfig);
            owner.TryGetConfig(out _quickstepConfig);
        }

        public override void OnEnter()
        {
            base.OnEnter();
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

            if (!Input.BHeld)
            {
                if (Input.moveVector.magnitude > 0.1f)
                {
                    StateMachine.SetState<FStateGround>();
                }
                else
                {
                    StateMachine.SetState<FStateIdle>();
                }
            }

            if (Input.JumpPressed)
            {
                StateMachine.SetState<FStateJump>(0.1f);
            }

            if (Input.moveVector.magnitude < 0.1)
            {
                StateMachine.SetState<FStateSit>();
            }

            if (_quickstepConfig)
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

            // how the hell do i make him move
            // you did it!!

            Vector3 prevNormal = Kinematics.Normal;
            BaseActorConfig config = Actor.config;
            float distance = config.castDistance * config.castDistanceCurve
                .Evaluate(Kinematics.HorizontalSpeed / _crawlConfig.topSpeed);
            if (Common.CheckForGround(out RaycastHit data, castDistance: distance) &&
                Kinematics.CheckForPredictedGround(_rigidbody.linearVelocity, Kinematics.Normal, Time.fixedDeltaTime, distance, 6))
            {
                Kinematics.Point = data.point;
                Kinematics.Normal = data.normal;

                Vector3 stored = Vector3.ClampMagnitude(_rigidbody.linearVelocity, _crawlConfig.maxSpeed);
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

        public string GetSurfaceTag() => _surfaceTag;
    }
}