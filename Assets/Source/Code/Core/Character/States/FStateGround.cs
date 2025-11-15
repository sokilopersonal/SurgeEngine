using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine.Source.Code.Infrastructure.Config;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public sealed class FStateGround : FCharacterState, IDamageableState
    {
        private WaterSurface _waterSurface;
        
        private const float BreakableThreshold = 0.7f; // If Speed Percent is greater than this, character will destroy Breakable objects

        public FStateGround(CharacterBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.SetDetachTime(0f);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Character.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.APressed)
                {
                    Kinematics.SetDetachTime(0.1f);
                    StateMachine.SetState<FStateJump>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Kinematics.Speed / Character.Config.topSpeed > BreakableThreshold)
            {
                HurtBox.CreateAttached(Character, Character.transform, new Vector3(0f, 0f, -0.1f), new Vector3(0.5f, 1f, 1.15f), HurtBoxTarget.Breakable);
            }

            PhysicsConfig config = Character.Config;
            float distance = config.EvaluateCastDistance(Kinematics.Speed / config.topSpeed);
            bool ground = Kinematics.CheckForGround(out RaycastHit data, castDistance: distance);
            bool isWater = false;
            if (data.transform != null)
            {
                isWater = data.transform.gameObject.GetGroundTag() == GroundTag.Water;
                if (isWater && data.transform.TryGetComponent(out _waterSurface))
                {
                    _waterSurface.Attach(Rigidbody.position, Character);
                    
                    if (Kinematics.HorizontalVelocity.magnitude < _waterSurface.MinimumSpeed)
                    {
                        StateMachine.SetState<FStateAir>();
                        return;
                    }
                }
            }
            
            bool predictedGround = Kinematics.CheckForPredictedGround(dt, distance, 6);
            if (ground)
            {
                Kinematics.Point = data.point;
                if (predictedGround) Kinematics.RotateSnapNormal(data.normal);
                else
                {
                    StateMachine.SetState<FStateSlip>();
                }
                
                Kinematics.ClampVelocityToMax();
                
                Kinematics.BasePhysics(Kinematics.Normal);
                if (!isWater)
                {
                    Kinematics.Snap(Kinematics.Point, Kinematics.Normal);
                }
                else
                {
                    Kinematics.SnapOnWater(data.point);
                }
                Model.RotateBody(Kinematics.Velocity, Kinematics.Normal);

                Kinematics.Project(Kinematics.Normal);
                Kinematics.SlopePhysics();

                Kinematics.GroundTag.Value = data.transform.gameObject.GetGroundTag();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }

            if (Kinematics.CheckForGroundWithDirection(out RaycastHit verticalHit, Rigidbody.transform.up) && Kinematics.Angle >= 90)
            {
                Kinematics.Point = verticalHit.point;
                Kinematics.Normal = verticalHit.normal;
            }
        }
    }

    public enum GroundTag
    {
        Grass,
        Concrete,
        Water
    }
}