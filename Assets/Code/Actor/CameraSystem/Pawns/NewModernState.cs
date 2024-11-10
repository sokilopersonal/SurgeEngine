using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class NewModernState : CState
    {
        private float _distance;

        private float _yOffset;

        private float _xAutoLook;
        
        private float _yLag;
        private float _yLagVelocity;
        private float _zLag;
        private float _zLagVelocity;

        public NewModernState(Camera camera, Transform transform, Actor owner) : base(camera, transform, owner)
        {
            _distance = 2.8f;
            _yOffset = 0.08f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            LookAxis();
            
            var actorPosition = CalculateTarget(out var targetPosition);

            YLag();
            ZLag();
            
            Setup(targetPosition, actorPosition);
        }

        private Vector3 CalculateTarget(out Vector3 targetPosition)
        {
            Quaternion horizontal = Quaternion.AngleAxis(_stateMachine.x, Vector3.up);
            Quaternion vertical = Quaternion.AngleAxis(_stateMachine.y, Vector3.right);
            Vector3 direction = horizontal * vertical * Vector3.back;
            Vector3 actorPosition = _actor.transform.position + Vector3.up * _yOffset + Vector3.up * _yLag;
            targetPosition = actorPosition + direction * (_distance + _zLag);
            return actorPosition;
        }

        private void LookAxis()
        {
            AutoLookDirection();
            
            var v = _actor.input.lookVector;
            _stateMachine.x += v.x + _xAutoLook;
            _stateMachine.y -= v.y;
            _stateMachine.y = Mathf.Clamp(_stateMachine.y, -75, 85);
        }

        private void YLag()
        {
            Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
            float yLag = Mathf.Clamp(vel.y * -0.1f, -0.5f, 0.5f); // min is down lag, max value is up lag
            _yLag = Mathf.SmoothDamp(_yLag, yLag, ref _yLagVelocity, 0.1f);
        }

        private void ZLag()
        {
            Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
            Vector3 localVel = _actor.transform.InverseTransformDirection(vel);
            float zLag = Mathf.Clamp(localVel.z * 0.075f, 0, 0.475f);
            _zLag = Mathf.SmoothDamp(_zLag, zLag, ref _zLagVelocity, 0.5f);
        }

        private void AutoLookDirection()
        {
            if (_actor.input.IsAutoCamera())
            {
                float speed = _actor.kinematics.HorizontalSpeed;
                if (speed > 1f)
                {
                    if (OnTheWall())
                    {
                        float fwd = _actor.stats.GetForwardSignedAngle() * Time.deltaTime;
                        float dot = Vector3.Dot(Vector3.Cross(_stateMachine.transform.right, Vector3.up), _actor.transform.forward);

                        if (!Mathf.Approximately(dot, -1))
                        {
                            _xAutoLook = fwd * 3;
                        }

                        Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
                        float yAutoLook = Mathf.Clamp(-vel.y * 1.25f, -15f, 15f);
                        _stateMachine.y = Mathf.Lerp(_stateMachine.y, yAutoLook, 0.1f);
                    }
                }
                else
                {
                    _xAutoLook = 0;
                }
            }
            else
            {
                _xAutoLook = 0;

            }
        }

        private void Setup(Vector3 targetPosition, Vector3 actorPosition)
        {
            _stateMachine.position = targetPosition;
            _stateMachine.rotation = Quaternion.LookRotation(actorPosition - _stateMachine.position);
        }

        private bool OnTheWall()
        {
            return !(1 - Mathf.Abs(Vector3.Dot(_actor.transform.forward, Vector3.up)) < 0.01f);
        }
    }
}
