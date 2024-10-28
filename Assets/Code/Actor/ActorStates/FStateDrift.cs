using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SonicSubStates.Boost;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using static SurgeEngine.Code.GameDocuments.SonicGameDocumentParams;

namespace SurgeEngine.Code.Parameters
{
    public class FStateDrift : FStateMove, IBoostHandler
    {
        private float _driftXDirection;
        private float _ignoreTimer;

        public override void OnEnter()
        {
            base.OnEnter();
            
            _ignoreTimer = 0;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (input.moveVector.x == 0)
                _ignoreTimer += dt;
            else
            {
                _ignoreTimer = 0;
            }
            
            if (!input.BHeld || _rigidbody.linearVelocity.magnitude < SonicGameDocument.GetDocument("Sonic").GetGroup("Drift").GetParameter<float>(Drift_DeactivateSpeed) || _ignoreTimer > 0.15f)
                stateMachine.SetState<FStateGround>(0.1f);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Common.CheckForGround(out var hit))
            {
                var point = hit.point;
                var normal = hit.normal;
                stats.groundNormal = normal;
                
                _rigidbody.position = point + normal;
                stats.transformNormal = Vector3.Slerp(stats.transformNormal, normal, dt * 14f);

                var param = SonicGameDocument.GetDocument("Sonic").GetGroup("Drift");
                
                _driftXDirection = Mathf.Lerp(_driftXDirection, input.moveVector.x, param.GetParameter<float>(Drift_Smoothness));
            
                actor.model.RotateBody(stats.groundNormal);
                
                stats.groundAngle = Vector3.Angle(stats.groundNormal, Vector3.up);
                if (stats.currentSpeed < 10 && stats.groundAngle >= 70)
                {
                    _rigidbody.AddForce(stats.groundNormal * 8f, ForceMode.Impulse);
                    stateMachine.SetState<FStateAir>(0.2f);
                }
            
                if (stats.groundAngle > 5 && stats.movementVector.magnitude > 10f)
                {
                    bool uphill = Vector3.Dot(_rigidbody.linearVelocity.normalized, Vector3.down) < 0;
                    Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, stats.groundNormal) * (1 * (uphill ? 1f : 50f));
                    _rigidbody.linearVelocity += slopeForce * Time.fixedDeltaTime;
                }
                
                float boostForce = stateMachine.GetSubState<FBoost>().Active ? 0.5f : 1f;
                Quaternion angle = Quaternion.AngleAxis(_driftXDirection * param.GetParameter<float>(Drift_CentrifugalForce) * boostForce, stats.groundNormal);
                Vector3 driftVelocity = angle * _rigidbody.linearVelocity;
                Vector3 additive = driftVelocity.normalized *
                                   ((1 - _rigidbody.linearVelocity.magnitude) * dt);
                if (additive.magnitude < param.GetParameter<float>(Drift_MaxSpeed) * 0.2f)
                    driftVelocity -= additive * 0.75f;
                _rigidbody.linearVelocity = driftVelocity;
                
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
            }
            else
            {
                stateMachine.SetState<FStateAir>(0.1f);
            }
        }

        public void BoostHandle()
        {
            float dt = Time.deltaTime;
            FBoost boost = stateMachine.GetSubState<FBoost>();
            var param = boost.GetBoostEnergyGroup();
            float startForce = param.GetParameter<float>(BoostEnergy_StartSpeed);
            if (boost.Active && stats.currentSpeed < startForce)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * startForce;
                boost.restoringTopSpeed = true;
            }
    
            if (boost.Active)
            {
                float maxSpeed = stats.moveParameters.maxSpeed * param.GetParameter<float>(BoostEnergy_MaxSpeedMultiplier);
                if (stats.currentSpeed < maxSpeed) _rigidbody.linearVelocity += _rigidbody.linearVelocity.normalized * (param.GetParameter<float>(BoostEnergy_Force) * dt);
                    
            }
            else if (boost.restoringTopSpeed)
            {
                float normalMaxSpeed = stats.moveParameters.topSpeed;
                if (stats.currentSpeed > normalMaxSpeed)
                {
                    _rigidbody.linearVelocity = Vector3.MoveTowards(
                        _rigidbody.linearVelocity, 
                        _rigidbody.transform.forward * normalMaxSpeed, 
                        dt * param.GetParameter<float>(BoostEnergy_RestoreSpeed)
                    );
                }
                else if (stats.currentSpeed * 0.99f < normalMaxSpeed)
                {
                    boost.restoringTopSpeed = false;
                }
            }
        }
    }
}