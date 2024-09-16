using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateAir : FStateMove
    {
        [SerializeField] private MoveParameters moveParameters;
        [SerializeField] private AirParameters airParameters;
        
        private Transform _cameraTransform;
        
        public override void OnEnter()
        {
            base.OnEnter();

            actor.stats.groundNormal = Vector3.up;
            _cameraTransform = actor.camera.GetCameraTransform();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (!Physics.Raycast(actor.transform.position, -actor.transform.up, out var hit,
                    1.25f, LayerMask.GetMask("Default")))
            {
                actor.stats.groundNormal = Vector3.up;
                
                Movement(dt, false);
                Rotate(dt);

                _rigidbody.linearVelocity += new Vector3(0, -airParameters.gravity, 0) * dt;
            }
            else
            {
                actor.stateMachine.SetState<FStateGround>();
            }
        }

        private void Movement(float dt, bool grounded)
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            var stats = actor.stats;
            var normal = stats.groundNormal;
            SurgeMath.SplitPlanarVector(velocity, normal, out Vector3 planar, out Vector3 airVelocity);

            stats.movementVector = planar;
            stats.planarVelocity = planar;

            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, normal) *
                                       (_cameraTransform.rotation * actor.input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, normal);
            stats.inputDir = transformedInput.normalized * actor.input.moveVector.magnitude;

            if (stats.inputDir.magnitude > 0.2f)
            {
                stats.turnRate = Mathf.Lerp(stats.turnRate, moveParameters.turnSpeed, dt * moveParameters.turnSmoothing);
                var accelRateMod = moveParameters.accelCurve.Evaluate(stats.planarVelocity.magnitude / moveParameters.topSpeed);
                if (stats.planarVelocity.magnitude < moveParameters.topSpeed)
                    stats.planarVelocity += stats.inputDir * (moveParameters.accelRate * accelRateMod * dt);
                float handling = stats.turnRate * 0.2f;
                handling *= moveParameters.turnCurve.Evaluate(stats.planarVelocity.magnitude / moveParameters.topSpeed);
                stats.movementVector = Vector3.Lerp(stats.planarVelocity, stats.inputDir.normalized * stats.planarVelocity.magnitude, 
                    dt * handling);
            }
            
            airVelocity = Vector3.ClampMagnitude(airVelocity, 125f);
            Vector3 movementVelocity = stats.movementVector + airVelocity;
            _rigidbody.linearVelocity = movementVelocity;
        }

        private void Rotate(float dt)
        {
            var stats = actor.stats;
            stats.transformNormal = Vector3.Slerp(stats.transformNormal, stats.groundNormal, dt * 3f);

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, stats.groundNormal);

            if (vel.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, stats.transformNormal);
                actor.transform.rotation = Quaternion.Slerp(actor.transform.rotation, rot, dt * 4f);
            }
        }
    }
}