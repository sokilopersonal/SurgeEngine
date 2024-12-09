using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.ActorStates
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
            var config = Actor.config;
            float distance = config.castDistance * config.castDistanceCurve
                .Evaluate(Kinematics.HorizontalSpeed / config.topSpeed);
            bool willBeGrounded = Kinematics.CheckForPredictedGround(_rigidbody.linearVelocity, prevNormal, dt, distance, 8);
            if (Common.CheckForGround(out var data, castDistance: distance) && willBeGrounded)
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
            Actor.stateMachine.GetSubState<FBoost>().BaseGroundBoost();
        }

        private void ConvertAirToGroundVelocity()
        {
            if (Physics.Raycast(Actor.transform.position, _rigidbody.linearVelocity.normalized, out RaycastHit velocityFix, _rigidbody.linearVelocity.magnitude, Actor.config.castLayer))
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