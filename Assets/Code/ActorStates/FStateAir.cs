using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateAir : FStateMove
    {
        [SerializeField] private MoveParameters moveParameters;
        [SerializeField] private AirParameters airParameters;
        
        private Vector3 _movementVector;
        private Vector3 _planarVelocity;

        private Transform _cameraTransform;

        private float _turnRate;
        
        private Vector3 _inputDir;
        private Vector3 _groundNormal;
        private Vector3 _transformNormal;
        
        public override void OnEnter()
        {
            base.OnEnter();

            _groundNormal = Vector3.up;
            _cameraTransform = actor.camera.GetCameraTransform();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (!Physics.Raycast(actor.transform.position, -actor.transform.up, out var hit,
                    1.25f, LayerMask.GetMask("Default")))
            {
                _groundNormal = Vector3.up;
                
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
            SurgeMath.SplitPlanarVector(velocity, _groundNormal, out Vector3 planar, out Vector3 airVelocity);

            _movementVector = planar;
            _planarVelocity = planar;

            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, _groundNormal) *
                                       (_cameraTransform.rotation * actor.input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, _groundNormal);
            _inputDir = transformedInput.normalized * actor.input.moveVector.magnitude;

            if (_inputDir.magnitude > 0.2f)
            {
                _turnRate = Mathf.Lerp(_turnRate, moveParameters.turnSpeed, dt * moveParameters.turnSmoothing);
                var accelRateMod = moveParameters.accelCurve.Evaluate(_planarVelocity.magnitude / moveParameters.topSpeed);
                if (_planarVelocity.magnitude < moveParameters.topSpeed)
                    _planarVelocity += _inputDir * (moveParameters.accelRate * accelRateMod * dt);
                float handling = _turnRate * 0.25f;
                handling *= moveParameters.turnCurve.Evaluate(_planarVelocity.magnitude / moveParameters.topSpeed);
                _movementVector = Vector3.Lerp(_planarVelocity, _inputDir.normalized * _planarVelocity.magnitude, 
                    dt * handling);
            }
            
            airVelocity = Vector3.ClampMagnitude(airVelocity, 125f);
            Vector3 movementVelocity = _movementVector + airVelocity;
            _rigidbody.linearVelocity = movementVelocity;
        }

        private void Rotate(float dt)
        {
            _transformNormal = Vector3.Slerp(_transformNormal, _groundNormal, dt * 4.5f);

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, _groundNormal);

            if (vel.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, _transformNormal);
                actor.transform.rotation = Quaternion.Slerp(actor.transform.rotation, rot, dt * 4);
            }
        }
    }
}