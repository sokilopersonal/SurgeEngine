using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateGround : FStateMove
    {
        [SerializeField] private float topSpeed;
        [SerializeField] private float turnSpeed;
        [SerializeField] private AnimationCurve turnCurve;
        [SerializeField] private float turnSmoothing;
        [SerializeField] private float accelRate;
        [SerializeField] private AnimationCurve accelCurve;
        [SerializeField] private Vector3 _groundCheckOffset;
        
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

            if (Physics.Raycast(actor.transform.position, -actor.transform.up, out var hit,
                    1.25f, LayerMask.GetMask("Default")))
            {
                _groundNormal = hit.normal;
                
                Movement(dt, true);
                Rotate(dt);
            }
            else
            {
                
            }
            
            //_rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, _groundNormal);
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
                _turnRate = Mathf.Lerp(_turnRate, turnSpeed, dt * turnSmoothing);
                var accelRateMod = accelCurve.Evaluate(_planarVelocity.magnitude / topSpeed);
                if (_planarVelocity.magnitude < topSpeed)
                    _planarVelocity += _inputDir * (accelRate * accelRateMod * dt);
                float handling = _turnRate;
                handling *= turnCurve.Evaluate(_planarVelocity.magnitude / topSpeed);
                _movementVector = Vector3.Lerp(_planarVelocity, _inputDir.normalized * _planarVelocity.magnitude, 
                    dt * handling);
            }
            else
            {
                float f = Mathf.Lerp(12, 4, _movementVector.magnitude / topSpeed);
                if (_movementVector.magnitude > 1f)
                    _movementVector = Vector3.MoveTowards(_movementVector, Vector3.zero, Time.fixedDeltaTime * f);
                else
                {
                    _movementVector = Vector3.zero;
                    
                    if (grounded) actor.stateMachine.SetState<FStateIdle>();
                }
            }
            
            airVelocity = Vector3.ClampMagnitude(airVelocity, 125f);
            Vector3 movementVelocity = _movementVector + airVelocity;
            _rigidbody.linearVelocity = movementVelocity;
        }

        private void Rotate(float dt)
        {
            _transformNormal = Vector3.Slerp(_transformNormal, _groundNormal, dt * 8);

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, _groundNormal);

            if (vel.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, _transformNormal);
                actor.transform.rotation = Quaternion.Slerp(actor.transform.rotation, rot, dt * 12);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + _groundCheckOffset, 0.1f);
            
            Gizmos.DrawRay(transform.position + _groundCheckOffset, -transform.up * 1.25f);
        }
    }
}