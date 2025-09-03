using System;
using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine._Source.Code.Infrastructure.Config;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public sealed class FStateGround : FCharacterState, IDamageableState
    {
        private WaterSurface _waterSurface;
        
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

            if (!character.Flags.HasFlag(FlagType.OutOfControl))
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

            PhysicsConfig config = character.Config;
            float distance = config.EvaluateCastDistance(Kinematics.Speed / config.topSpeed);
            bool ground = Kinematics.CheckForGround(out RaycastHit data, castDistance: distance);
            bool isWater = false;
            if (data.transform != null)
            {
                isWater = data.transform.gameObject.GetGroundTag() == GroundTag.Water;
                if (isWater && data.transform.TryGetComponent(out _waterSurface))
                {
                    _waterSurface.Attach(Rigidbody.position, character);
                    
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
                    Kinematics.Normal = Vector3.up;
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
                Model.RotateBodyInput(Kinematics.Normal, 1200);

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