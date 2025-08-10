using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.Actor.System.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.Inputs;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateDrift : FCharacterState, IStateTimeout, IDamageableState
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
            
            if (Input.moveVector.x == 0)
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
            HurtBox.CreateAttached(character, transform, offset, new Vector3(0.75f, 0.3f, 0.75f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
            
            var physicsConfig = character.Config;
            float distance = physicsConfig.castDistance * physicsConfig.castDistanceCurve.Evaluate(Kinematics.Speed / physicsConfig.topSpeed);
            if (Kinematics.CheckForGround(out RaycastHit hit, castDistance: distance))
            {
                bool predictedGround = Kinematics.CheckForPredictedGround(hit.normal, dt, character.Config.castDistance, 4);
                Vector3 point = hit.point;
                Vector3 targetNormal = hit.normal;
                if (predictedGround)
                {
                    targetNormal = Vector3.up;
                    Kinematics.Normal = targetNormal;
                }
                
                if (!predictedGround) Kinematics.RotateSnapNormal(targetNormal);
                
                Vector3 dir = Input.moveVector;
                _driftXDirection = Mathf.Sign(dir.x);

                Model.RotateBody(Kinematics.Velocity, targetNormal, 1600);
                
                Kinematics.SlopePhysics();
                
                float boostForce = SonicTools.IsBoost() ? 0.5f : 1f;
                Quaternion angle = Quaternion.AngleAxis(_driftXDirection * _config.centrifugalForce * boostForce, targetNormal);
                Vector3 driftVelocity = angle * Rigidbody.linearVelocity;
                Rigidbody.linearVelocity = driftVelocity;
                if (Kinematics.Speed < _config.maxSpeed) Rigidbody.linearVelocity += Rigidbody.linearVelocity.normalized *
                    (0.05f * Mathf.Lerp(4f, 1f, Kinematics.Speed / _config.maxSpeed));

                Kinematics.Snap(point, targetNormal);
                Kinematics.Project(targetNormal);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public float Timeout { get; set; }
    }
}