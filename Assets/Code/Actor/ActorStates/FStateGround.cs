using SurgeEngine.Code.ActorSystem;
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
        private string _surfaceTag;
        private const float EdgePushForce = 3.5f;

        public FStateGround(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
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
            if (Common.CheckForGround(out var data))
            {
                var point = data.point;
                Kinematics.Normal = data.normal;
                
                Vector3 stored = _rigidbody.linearVelocity;
                _rigidbody.linearVelocity = Quaternion.FromToRotation(_rigidbody.transform.up, prevNormal) * stored;
                Stats.transformNormal = Vector3.Slerp(Stats.transformNormal, Kinematics.Normal, dt * 14f);

                Actor.kinematics.BasePhysics(point, Kinematics.Normal);
                Actor.model.RotateBody(Kinematics.Normal);
                
                _surfaceTag = data.transform.gameObject.GetGroundTag();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public void BoostHandle()
        {
            float dt = Time.deltaTime;
            FBoost boost = StateMachine.GetSubState<FBoost>();
            var phys = SonicGameDocument.GetDocument("Sonic").GetGroup(SonicGameDocument.PhysicsGroup);
            var param = boost.GetBoostEnergyGroup();
            float startForce = param.GetParameter<float>(BoostEnergy_StartSpeed);
            if (Input.BoostPressed && Stats.currentSpeed < startForce)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * startForce;
                boost.restoringTopSpeed = true;
            }
    
            if (boost.Active)
            {
                float maxSpeed = phys.GetParameter<float>(BasePhysics_MaxSpeed) * param.GetParameter<float>(BoostEnergy_MaxSpeedMultiplier);
                if (Stats.currentSpeed < maxSpeed) _rigidbody.AddForce(_rigidbody.transform.forward * (param.GetParameter<float>(BoostEnergy_Force) * dt), ForceMode.VelocityChange);
                    
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
    }
}