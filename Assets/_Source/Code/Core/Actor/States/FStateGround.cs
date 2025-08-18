using System;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SurgeEngine.Code.Core.Actor.States
{
    public sealed class FStateGround : FCharacterState, IDamageableState
    {
        private GroundTag _surfaceTag;
        
        private WaterSurface _waterSurface;
        private Vector3 _waterSnapVelocity;

        public event Action<GroundTag> OnSurfaceTagChanged;
        
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
            
            if (ground)
            {
                bool predictedGround = Kinematics.CheckForPredictedGround(data.normal, dt, distance, 8);
                Vector3 targetNormal = data.normal;
                if (predictedGround)
                {
                    targetNormal = Vector3.up;
                    Kinematics.Normal = targetNormal;
                }
                
                Kinematics.Point = data.point;
                Kinematics.RotateSnapNormal(targetNormal);
                
                Kinematics.ClampVelocityToMax();
                
                Kinematics.BasePhysics(Kinematics.Normal);
                if (!isWater)
                {
                    Kinematics.Snap(Kinematics.Point, Kinematics.Normal);
                    _waterSnapVelocity = Vector3.zero;
                }
                else
                {
                    var point = data.point;
                    point.y -= 0.1f;
                    var normal = Kinematics.Normal;
                    var end = Vector3.SmoothDamp(Rigidbody.position, point + normal, ref _waterSnapVelocity, 0.5f);
                    Kinematics.Snap(end);
                }
                Model.RotateBody(Kinematics.Velocity, Kinematics.Normal);

                Kinematics.Project(Kinematics.Normal);
                
                if (Kinematics.Speed < config.topSpeed / 4 && Kinematics.IsHardAngle(targetNormal) && Kinematics.mode == KinematicsMode.Free)
                {
                    StateMachine.SetState<FStateSlip>();
                }
                
                Kinematics.SlopePhysics();
                
                UpdateSurfaceTag(data.transform.gameObject.GetGroundTag());
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

        private void UpdateSurfaceTag(GroundTag newTag)
        {
            if (_surfaceTag != newTag)
            {
                _surfaceTag = newTag;
                OnSurfaceTagChanged?.Invoke(newTag);
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