using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SonicSubStates.Boost;
using UnityEngine;
using UnityEngine.Splines;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using static SurgeEngine.Code.GameDocuments.SonicGameDocumentParams;

namespace SurgeEngine.Code.Parameters
{
    public sealed class FStateGround : FStateMove, IBoostHandler
    {
        [SerializeField] private Vector3 _groundCheckOffset;
        
        
        private string _surfaceTag;

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.Normal = Vector3.up;
            
            Kinematics.SetDetachTime(0f);
            ConvertAirToGroundVelocity();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Actor.input.JumpPressed)
            {
                Actor.stateMachine.SetState<FStateJump>(0.1f);
            }

            float activateSpeed = StateMachine.GetState<FStateSliding>().slideDeactivationSpeed;
            activateSpeed += activateSpeed * 1.5f;
            
            if (Input.BHeld)
            {
                float dot = Stats.moveDot;
                float abs = Mathf.Abs(dot);
                bool allowDrift = Stats.currentSpeed > 10 && abs < 0.4f && !Mathf.Approximately(dot, 0f);
                bool allowSlide = Stats.currentSpeed > activateSpeed;
                    
                if (allowDrift)
                    StateMachine.SetState<FStateDrift>();
                else if (allowSlide)
                    StateMachine.SetState<FStateSliding>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Vector3 prevNormal = Kinematics.Normal; 
            if (Common.CheckForGround(out var hit))
            {
                var point = hit.point;
                Kinematics.Normal = hit.normal;
                
                Vector3 stored = _rigidbody.linearVelocity;
                _rigidbody.linearVelocity = Quaternion.FromToRotation(_rigidbody.transform.up, prevNormal) * stored;
                Stats.transformNormal = Vector3.Slerp(Stats.transformNormal, Kinematics.Normal, dt * 14f);

                Actor.kinematics.BasePhysics(point, Kinematics.Normal);
                Actor.model.RotateBody(Kinematics.Normal);
                
                _surfaceTag = hit.transform.gameObject.GetGroundTag();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
            
            // TODO: Move to ActorKinematics
            var path = Actor.pathData;
            if (path != null)
            {
                if (path.splineContainer != null)
                {
                    var container = path.splineContainer;
                    SplineUtility.GetNearestPoint(container.Spline, SurgeMath.Vector3ToFloat3(container.transform.InverseTransformPoint(_rigidbody.position)), out var near, out var t);
                    container.Evaluate(t, out var point, out var tangent, out var up);
                    var planeNormal = Vector3.Cross(tangent, up);
                
                    if (Stats.currentSpeed < path.maxAutoRunSpeed)
                    {
                        _rigidbody.AddForce(_rigidbody.transform.forward * (dt * path.autoRunSpeed), ForceMode.Impulse);
                    }
                
                    _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, planeNormal);
                    Stats.inputDir = Vector3.ProjectOnPlane(Stats.inputDir, planeNormal);
                
                    Vector3 nearPoint = container.transform.TransformPoint(near);
                    _rigidbody.position = Vector3.Lerp(_rigidbody.position, nearPoint, 16f * dt);
                    _rigidbody.rotation = Quaternion.LookRotation(tangent, up);
                }
            }
        }

        public void BoostHandle()
        {
            float dt = Time.deltaTime;
            FBoost boost = StateMachine.GetSubState<FBoost>();
            var param = boost.GetBoostEnergyGroup();
            float startForce = param.GetParameter<float>(BoostEnergy_StartSpeed);
            if (boost.Active && Stats.currentSpeed < startForce)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * startForce;
                boost.restoringTopSpeed = true;
            }
    
            if (boost.Active)
            {
                float maxSpeed = Stats.moveParameters.maxSpeed * param.GetParameter<float>(BoostEnergy_MaxSpeedMultiplier);
                if (Stats.currentSpeed < maxSpeed) _rigidbody.linearVelocity += _rigidbody.linearVelocity.normalized * (param.GetParameter<float>(BoostEnergy_Force) * dt);
                    
            }
            else if (boost.restoringTopSpeed)
            {
                float normalMaxSpeed = Stats.moveParameters.topSpeed;
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

        private void ConvertAirToGroundVelocity()
        {
            if (Physics.Raycast(Actor.transform.position, _rigidbody.linearVelocity.normalized, out RaycastHit velocityFix, _rigidbody.linearVelocity.magnitude, Stats.moveParameters.castParameters.collisionMask))
            {
                float nextGroundAngle = Vector3.Angle(velocityFix.normal, Vector3.up);
                if (nextGroundAngle <= 20)
                {
                    Vector3 fixedVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, Actor.transform.up);
                    fixedVelocity = Quaternion.FromToRotation(Actor.transform.up, velocityFix.normal) * fixedVelocity;
                    _rigidbody.linearVelocity = fixedVelocity;
                }
            }
        }

        public string GetSurfaceTag() => _surfaceTag;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + _groundCheckOffset, 0.1f);
            
            Gizmos.DrawRay(transform.position + _groundCheckOffset, -transform.up * 1.5f);
        }
    }
}