using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Inputs;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.Tools;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateDrift : FStateMove, IBoostHandler, IStateTimeout
    {
        private float _driftXDirection;
        private float _ignoreTimer;

        private readonly DriftConfig _config;

        public FStateDrift(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _config = (owner as Sonic).driftConfig;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Timeout = 0.75f;
            _ignoreTimer = 0;
        }

        public override void OnExit()
        {
            base.OnExit();
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
            
            if (!SonicInputLayout.DriftHeld || _rigidbody.linearVelocity.magnitude < _config.minSpeed|| _ignoreTimer > 0.15f)
                StateMachine.SetState<FStateGround>(0.1f);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            HurtBox.Create(Actor, Actor.transform.position + new Vector3(0f, -0.75f, 0f), Actor.transform.rotation,
                new Vector3(0.75f, 0.3f, 0.75f));
            
            if (Common.CheckForGround(out RaycastHit hit))
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
                Vector3 driftVelocity = angle * _rigidbody.linearVelocity;
                _rigidbody.linearVelocity = driftVelocity;
                if (Kinematics.HorizontalSpeed < _config.maxSpeed) _rigidbody.linearVelocity += _rigidbody.linearVelocity.normalized *
                    (0.05f * Mathf.Lerp(4f, 1f, Kinematics.HorizontalSpeed / _config.maxSpeed));

                Kinematics.Snap(point, normal);
                Kinematics.Project();
            }
            else
            {
                StateMachine.SetState<FStateAir>(0.1f);
            }
        }

        public void BoostHandle()
        {
            StateMachine.GetSubState<FBoost>().BaseGroundBoost();
        }

        public float Timeout { get; set; }
    }
}