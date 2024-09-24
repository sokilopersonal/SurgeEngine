using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateAir : FStateMove
    {
        [SerializeField] private AirParameters airParameters;
        
        private Transform _cameraTransform;

        private float _airTime;
        
        public override void OnEnter()
        {
            base.OnEnter();

            _airTime = 0f;

            if (!stateMachine.IsPreviousState<FStateAirBoost>()) 
                animation.TransitionToState("Air Cycle", 0f);
            
            if (stats.lastContactObject is JumpCollision)
            {
                animation.TransitionToState("Air Cycle", 0.2f);
                stats.lastContactObject = null;
            }
            
            actor.stats.groundNormal = Vector3.up;
            _cameraTransform = actor.camera.GetCameraTransform();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            CalculateAirTime(dt);
            stateMachine.GetState<FStateGround>().CalculateDetachState();

            if (input.BoostPressed && stateMachine.GetSubState<FBoost>().CanBoost())
            {
                stateMachine.SetState<FStateAirBoost>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (!Common.CheckForGround(out _))
            {
                stats.groundNormal = Vector3.up;
                
                Movement(dt);
                Rotate(dt);

                Common.ApplyGravity(airParameters.gravity, dt);
            }
            else
            {
                if (stateMachine.GetState<FStateGround>().GetAttachState()) stateMachine.SetState<FStateGround>();
            }
        }

        private void Movement(float dt)
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            var normal = stats.groundNormal;
            SurgeMath.SplitPlanarVector(velocity, normal, out Vector3 planar, out Vector3 airVelocity);

            stats.movementVector = planar;
            stats.planarVelocity = planar;

            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, normal) *
                                       (_cameraTransform.rotation * input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, normal);
            stats.inputDir = transformedInput.normalized * input.moveVector.magnitude;

            if (stats.inputDir.magnitude > 0.2f)
            {
                stats.turnRate = Mathf.Lerp(stats.turnRate, stats.moveParameters.turnSpeed, dt * stats.moveParameters.turnSmoothing);
                var accelRateMod = stats.moveParameters.accelCurve.Evaluate(stats.planarVelocity.magnitude / stats.moveParameters.topSpeed);
                if (stats.planarVelocity.magnitude < stats.moveParameters.topSpeed)
                    stats.planarVelocity += stats.inputDir * (stats.moveParameters.accelRate * accelRateMod * dt);
                float handling = stats.turnRate * 0.2f;
                //handling *= stats.moveParameters.turnCurve.Evaluate(stats.planarVelocity.magnitude / stats.moveParameters.topSpeed);
                stats.movementVector = Vector3.Lerp(stats.planarVelocity, stats.inputDir.normalized * stats.planarVelocity.magnitude, 
                    dt * handling);
            }
            
            airVelocity = Vector3.ClampMagnitude(airVelocity, 125f);
            Vector3 movementVelocity = stats.movementVector + airVelocity;
            _rigidbody.linearVelocity = movementVelocity;
        }

        private void Rotate(float dt)
        {
            stats.transformNormal = Vector3.Slerp(stats.transformNormal, Vector3.up, dt * 5f);

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, stats.groundNormal);

            if (vel != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(vel, stats.transformNormal);
                actor.transform.rotation = rot;
            }
        }

        protected float GetAirTime() => _airTime;
        
        private void CalculateAirTime(float dt)
        {
            _airTime += dt;
        }
    }
}