using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Tools;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateDrift : FStateMove, IBoostHandler
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
            
            _ignoreTimer = 0;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Animation.ResetAction();
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
            
            if (!Input.BHeld || _rigidbody.linearVelocity.magnitude < _config.minSpeed|| _ignoreTimer > 0.15f)
                StateMachine.SetState<FStateGround>(0.1f);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Common.CheckForGround(out var hit))
            {
                var point = hit.point;
                var normal = hit.normal;
                Kinematics.Normal = normal;
                
                Vector3 dir = Input.moveVector;
                _driftXDirection = Mathf.Sign(dir.x);
            
                Actor.model.RotateBody(Kinematics.Normal);
                
                Kinematics.SlopePhysics();
                
                float boostForce = SonicTools.IsBoost() ? 0.5f : 1f;
                Quaternion angle = Quaternion.AngleAxis(_driftXDirection * _config.centrifugalForce * boostForce, Kinematics.Normal);
                Vector3 driftVelocity = angle * _rigidbody.linearVelocity;
                Vector3 additive = driftVelocity.normalized *
                                   (1 - _rigidbody.linearVelocity.magnitude);
                if (additive.magnitude < _config.maxSpeed * 0.2f)
                    driftVelocity -= additive * 0.75f;
                _rigidbody.linearVelocity = driftVelocity;

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
    }
}