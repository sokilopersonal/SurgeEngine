using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using static SurgeEngine.Code.GameDocuments.SonicGameDocumentParams;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateDrift : FStateMove, IBoostHandler
    {
        private float _driftXDirection;
        private float _ignoreTimer;

        public FStateDrift(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
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
            
            if (!Input.BHeld || _rigidbody.linearVelocity.magnitude < SonicGameDocument.GetDocument("Sonic").GetGroup("Drift").GetParameter<float>(Drift_DeactivateSpeed) || _ignoreTimer > 0.15f)
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

                var param = SonicGameDocument.GetDocument("Sonic").GetGroup("Drift");
                Vector3 dir = Input.moveVector;
                _driftXDirection = Mathf.Sign(dir.x);
            
                Actor.model.RotateBody(Kinematics.Normal);
                
                Kinematics.SlopePhysics();
                
                float boostForce = StateMachine.GetSubState<FBoost>().Active ? 0.5f : 1f;
                Quaternion angle = Quaternion.AngleAxis(_driftXDirection * param.GetParameter<float>(Drift_CentrifugalForce) * boostForce, Kinematics.Normal);
                Vector3 driftVelocity = angle * _rigidbody.linearVelocity;
                Vector3 additive = driftVelocity.normalized *
                                   (1 - _rigidbody.linearVelocity.magnitude);
                if (additive.magnitude < param.GetParameter<float>(Drift_MaxSpeed) * 0.2f)
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
            float dt = Time.deltaTime;
            FBoost boost = StateMachine.GetSubState<FBoost>();
            var physParam = SonicGameDocument.GetDocument("Sonic").GetGroup(SonicGameDocument.PhysicsGroup);
            var param = boost.GetBoostEnergyGroup();
            float startForce = param.GetParameter<float>(BoostEnergy_StartSpeed);
            if (boost.Active && Stats.currentSpeed < startForce)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * startForce;
                boost.restoringTopSpeed = true;
            }
    
            if (boost.Active)
            {
                float maxSpeed = physParam.GetParameter<float>(BasePhysics_MaxSpeed) * param.GetParameter<float>(BoostEnergy_MaxSpeedMultiplier);
                if (Stats.currentSpeed < maxSpeed) _rigidbody.linearVelocity += _rigidbody.linearVelocity.normalized * (param.GetParameter<float>(BoostEnergy_Force) * dt);
            }
            else if (boost.restoringTopSpeed)
            {
                float normalMaxSpeed = physParam.GetParameter<float>(BasePhysics_TopSpeed);
                if (Stats.currentSpeed > normalMaxSpeed)
                {
                    _rigidbody.linearVelocity = Vector3.MoveTowards(
                        _rigidbody.linearVelocity, 
                        _rigidbody.transform.forward * normalMaxSpeed, 
                        dt * param.GetParameter<float>(BoostEnergy_RestoreSpeed)
                    );
                }
                else if (Stats.currentSpeed * 0.99f < normalMaxSpeed)
                {
                    boost.restoringTopSpeed = false;
                }
            }
        }
    }
}