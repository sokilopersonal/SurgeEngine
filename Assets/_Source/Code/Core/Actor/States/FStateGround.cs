using System;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
            ConvertAirToGroundVelocity();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Actor.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.APressed)
                {
                    StateMachine.SetState<FStateJump>(0.1f);
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Vector3 prevNormal = Kinematics.Normal;
            BaseActorConfig config = Actor.Config;
            float distance = config.castDistance * config.castDistanceCurve
                .Evaluate(Kinematics.HorizontalSpeed / config.topSpeed);
            bool checkForPredictedGround =
                Kinematics.CheckForPredictedGround(Kinematics.Velocity, Rigidbody.transform.up, Time.fixedDeltaTime, distance, 8);
            bool ground = Kinematics.CheckForGround(out RaycastHit data, castDistance: distance);
            if (ground && checkForPredictedGround)
            {
                Kinematics.Point = data.point;
                Kinematics.RotateSnapNormal(data.normal);
                
                Vector3 stored = Vector3.ClampMagnitude(Rigidbody.linearVelocity, config.maxSpeed);
                //Rigidbody.linearVelocity = Quaternion.FromToRotation(Rigidbody.transform.up, prevNormal) * stored;
                
                Kinematics.BasePhysics(Kinematics.Normal);
                Kinematics.Snap(Kinematics.Point, Kinematics.Normal, true);
                Model.RotateBody(Kinematics.Velocity, Kinematics.Normal);
                Kinematics.Project(Kinematics.Normal);
                
                bool hit = Physics.Raycast(Rigidbody.position, Rigidbody.transform.forward, 0.5f, config.castLayer);
                if (Kinematics.Speed < config.topSpeed / 4 && !hit && Kinematics.IsHardAngle(data.normal) && Kinematics.mode == KinematicsMode.Free)
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
                //Kinematics.Point = verticalHit.point;
                //Kinematics.Normal = verticalHit.normal;
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

        private void ConvertAirToGroundVelocity()
        {
            if (Physics.Raycast(Actor.transform.position, Rigidbody.linearVelocity.normalized, out RaycastHit velocityFix, Rigidbody.linearVelocity.magnitude, Actor.Config.castLayer))
            {
                float nextGroundAngle = Vector3.Angle(velocityFix.normal, Vector3.up);
                if (nextGroundAngle <= Kinematics.maxAngleDifference)
                {
                    Vector3 fixedVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, Kinematics.Normal);
                    fixedVelocity = Quaternion.FromToRotation(Actor.transform.up, velocityFix.normal) * fixedVelocity;
                    Rigidbody.linearVelocity = fixedVelocity;
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