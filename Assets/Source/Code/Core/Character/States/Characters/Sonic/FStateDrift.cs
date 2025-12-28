using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic;
using SurgeEngine.Source.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic
{
    public class FStateDrift : FCharacterState, IStateTimeout, IDamageableState, IWaterMaintainSpeed
    {
        private float _driftXDirection;
        private float _ignoreTimer;

        private readonly DriftConfig _config;

        public FStateDrift(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Timeout = 0.5f;
            _ignoreTimer = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Input.MoveVector.x == 0)
                _ignoreTimer += dt;
            else
            {
                _ignoreTimer = 0;
            }
            
            bool driftHeld = ((SonicInput)Input).DriftHeld;
            if (!driftHeld || _ignoreTimer > 0.15f)
                StateMachine.SetState<FStateGround>();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            var transform = Rigidbody.transform;
            var offset = -transform.up * 0.75f;
            HurtBox.CreateAttached(Character, transform, offset, new Vector3(0.75f, 0.3f, 0.75f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
            
            var physicsConfig = Character.Config;
            float distance = physicsConfig.castDistance * physicsConfig.castDistanceCurve.Evaluate(Kinematics.Speed / physicsConfig.topSpeed);
            var ground = Kinematics.CheckForGround(out RaycastHit hit, castDistance: distance);
            bool isWater = hit.transform.IsWater(out var surface);
            if (isWater)
            {
                surface.Attach(Rigidbody.position, Character);
                    
                if (Kinematics.HorizontalVelocity.magnitude < surface.MinimumSpeed)
                {
                    StateMachine.SetState<FStateAir>();
                    return;
                }
            }
            
            bool predictedGround = Kinematics.CheckForPredictedGround(dt, Character.Config.castDistance, 4);
            if (ground && predictedGround)
            {
                Vector3 point = hit.point;
                Vector3 targetNormal = hit.normal;
                Kinematics.RotateSnapNormal(targetNormal);
                
                Vector3 dir = Input.MoveVector;
                _driftXDirection = Mathf.Sign(dir.x);

                Model.RotateBody(Kinematics.Normal);
                
                Kinematics.SlopePhysics();
                
                float force = 1f;
                if (Character.StateMachine.GetState(out FBoost boost) && boost.Active)
                    force *= 0.66f;
                
                Quaternion angle = Quaternion.AngleAxis(_driftXDirection * _config.centrifugalForce * force, Kinematics.Normal);
                Vector3 driftVelocity = angle * Rigidbody.linearVelocity;
                Rigidbody.linearVelocity = driftVelocity;
                if (Kinematics.Speed < _config.maxSpeed) Rigidbody.linearVelocity += Rigidbody.linearVelocity.normalized *
                    (0.05f * Mathf.Lerp(4f, 1f, Kinematics.Speed / _config.maxSpeed));
                
                if (!isWater) Kinematics.Snap(point, Kinematics.Normal);
                else Kinematics.SnapOnWater(point);
                
                Kinematics.Project(Kinematics.Normal);
                Kinematics.GroundTag.Value = hit.transform.gameObject.GetGroundTag();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public float Timeout { get; set; }
    }
}