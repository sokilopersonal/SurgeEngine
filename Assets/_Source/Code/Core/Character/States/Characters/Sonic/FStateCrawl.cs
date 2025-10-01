using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Interfaces;
using SurgeEngine._Source.Code.Infrastructure.Config;
using SurgeEngine._Source.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic
{
    public class FStateCrawl : FCharacterState, IStateTimeout, IDamageableState
    {
        private readonly CrawlConfig _crawlConfig;
        
        public FStateCrawl(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _crawlConfig);
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
                if (Input.MoveVector.magnitude > 0.1f)
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

            if (Input.MoveVector.magnitude < 0.1)
            {
                StateMachine.SetState<FStateSit>();
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
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
        
        public float Timeout { get; set; }
    }
}