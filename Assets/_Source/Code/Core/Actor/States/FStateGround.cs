using System;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SurgeEngine.Code.Core.Actor.States
{
    public sealed class FStateGround : FActorState, IDamageableState
    {
        private GroundTag _surfaceTag;
        private float _hardAngleTimer;

        public event Action<GroundTag> OnSurfaceTagChanged;
        
        public FStateGround(ActorBase owner) : base(owner)
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

            if (!Actor.Flags.HasFlag(FlagType.OutOfControl))
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

            PhysicsConfig config = Actor.Config;
            float distance = config.EvaluateCastDistance(Kinematics.Speed / config.topSpeed);
            bool ground = Kinematics.CheckForGround(out RaycastHit data, castDistance: distance);
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
                Kinematics.Snap(Kinematics.Point, Kinematics.Normal, true);
                Model.RotateBody(Kinematics.Velocity, Kinematics.Normal);

                Rigidbody.linearVelocity -= Vector3.Project(Rigidbody.linearVelocity, Kinematics.Normal);
                
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