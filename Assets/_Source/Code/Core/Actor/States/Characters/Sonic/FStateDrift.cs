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
    public class FStateDrift : FActorState, IStateTimeout, IDamageableState
    {
        private float _driftXDirection;
        private float _ignoreTimer;

        private readonly DriftConfig _config;

        public FStateDrift(ActorBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Timeout = 0.75f;
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
            if (!driftHeld || Rigidbody.linearVelocity.magnitude < _config.minSpeed|| _ignoreTimer > 0.15f)
                StateMachine.SetState<FStateGround>(0.1f);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            var transform = Rigidbody.transform;
            var offset = -transform.up * 0.75f;
            HurtBox.CreateAttached(Actor, transform, offset, new Vector3(0.75f, 0.3f, 0.75f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
            
            if (Kinematics.CheckForGround(out RaycastHit hit))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;
                Kinematics.Normal = normal;
                
                Vector3 dir = Input.moveVector;
                _driftXDirection = Mathf.Sign(dir.x);
            
                Model.RotateBody(Kinematics.Normal);
                
                Kinematics.SlopePhysics();
                
                float boostForce = SonicTools.IsBoost() ? 0.5f : 1f;
                Quaternion angle = Quaternion.AngleAxis(_driftXDirection * _config.centrifugalForce * boostForce, Kinematics.Normal);
                Vector3 driftVelocity = angle * Rigidbody.linearVelocity;
                Rigidbody.linearVelocity = driftVelocity;
                if (Kinematics.HorizontalSpeed < _config.maxSpeed) Rigidbody.linearVelocity += Rigidbody.linearVelocity.normalized *
                    (0.05f * Mathf.Lerp(4f, 1f, Kinematics.HorizontalSpeed / _config.maxSpeed));

                Kinematics.Snap(point, normal);
                Kinematics.Project();
            }
            else
            {
                StateMachine.SetState<FStateAir>(0.1f);
            }
        }

        public float Timeout { get; set; }
    }
}