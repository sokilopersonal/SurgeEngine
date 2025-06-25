using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateCrawl : FActorState, IStateTimeout, IDamageableState
    {
        private GroundTag _surfaceTag;

        private readonly CrawlConfig _crawlConfig;
        private readonly QuickStepConfig _quickstepConfig;
        
        public FStateCrawl(ActorBase owner) : base(owner)
        {
            owner.TryGetConfig(out _crawlConfig);
            owner.TryGetConfig(out _quickstepConfig);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            StateMachine.GetSubState<FBoost>().Active = false;

            Timeout = 0.5f;
            
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
            if (!Input.BHeld && !Kinematics.CheckForCeiling(out RaycastHit data))
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

            if (Input.APressed)
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
            PhysicsConfig config = Actor.Config;
            float distance = config.castDistance * config.castDistanceCurve
                .Evaluate(Kinematics.HorizontalSpeed / _crawlConfig.topSpeed);
            if (Kinematics.CheckForGroundWithDirection(out RaycastHit data, Vector3.down, distance))
            {
                Kinematics.Point = data.point;
                Kinematics.Normal = Vector3.up;

                Vector3 stored = Vector3.ClampMagnitude(Rigidbody.linearVelocity, _crawlConfig.maxSpeed);
                Rigidbody.linearVelocity = Quaternion.FromToRotation(Rigidbody.transform.up, prevNormal) * stored;

                Kinematics.BasePhysics(data.normal);
                
                Kinematics.Snap(Kinematics.Point, Vector3.up, true);
                Model.RotateBody(Vector3.up);

                _surfaceTag = data.transform.gameObject.GetGroundTag();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public GroundTag GetSurfaceTag() => _surfaceTag;
        public float Timeout { get; set; }
    }
}