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
    public class FStateCrawl : FCharacterState, IStateTimeout, IDamageableState
    {
        private GroundTag _surfaceTag;

        private readonly CrawlConfig _crawlConfig;
        private readonly QuickStepConfig _quickstepConfig;
        
        public FStateCrawl(CharacterBase owner) : base(owner)
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
                StateMachine.SetState<FStateJump>();
            }

            if (Input.moveVector.magnitude < 0.1)
            {
                StateMachine.SetState<FStateSit>();
            }

            if (_quickstepConfig)
            {
                if (Input.LeftBumperPressed)
                {
                    var qs = StateMachine.GetState<FStateQuickstep>();
                    qs.SetDirection(QuickstepDirection.Left).SetRun(false);
                    StateMachine.SetState<FStateQuickstep>();
                }
                else if (Input.RightBumperPressed)
                {
                    var qs = StateMachine.GetState<FStateQuickstep>();
                    qs.SetDirection(QuickstepDirection.Right).SetRun(false);
                    StateMachine.SetState<FStateQuickstep>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            // how the hell do i make him move
            // you did it!!
            PhysicsConfig config = character.Config;
            float distance = config.castDistance * config.castDistanceCurve
                .Evaluate(Kinematics.Speed / _crawlConfig.topSpeed);
            if (Kinematics.CheckForGroundWithDirection(out RaycastHit data, Vector3.down, distance))
            {
                Kinematics.Point = data.point;
                Kinematics.Normal = Vector3.up;

                Kinematics.ClampVelocityToMax(_crawlConfig.maxSpeed);

                Kinematics.BasePhysics(Kinematics.Normal);
                Kinematics.Project(Kinematics.Normal);
                
                Kinematics.Snap(Kinematics.Point, Vector3.up);
                Model.RotateBody(Vector3.up);

                _surfaceTag = data.transform.gameObject.GetGroundTag();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
        
        public float Timeout { get; set; }
    }
}