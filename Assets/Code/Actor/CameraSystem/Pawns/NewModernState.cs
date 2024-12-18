using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class NewModernState : CState
    {
        protected float _distance;
        protected float _yOffset;
        private float _sensSpeedMod;
        private Vector3 _velocity;
        private float _lookVelocity;
        private float _currentCollisionDistance;

        private float _boostDistance;
        private float _lookYTime;
        private float _yAutoLookVelocity;
        private float _xAutoLookVelocity;

        private bool _isAuto => _actor.input.IsAutoCamera();

        public NewModernState(Actor owner) : base(owner)
        {
            _distance = _master.distance;
            _yOffset = _master.yOffset;

            _boostDistance = 1f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            LookAxis();
            ModernSetup();
        }

        private void ModernSetup()
        {
            Vector3 actorPosition = CalculateTarget(out Vector3 targetPosition, _distance * _boostDistance, _yOffset);

            ZLag();
            YLag();
            
            Setup(targetPosition, actorPosition);
        }

        private Vector3 CalculateTarget(out Vector3 targetPosition, float distance, float yOffset)
        {
            Quaternion horizontal = Quaternion.AngleAxis(_stateMachine.x, Vector3.up);
            Quaternion vertical = Quaternion.AngleAxis(_stateMachine.y, Vector3.right);
            Vector3 direction = horizontal * vertical * Vector3.back;
            Vector3 actorPosition = _actor.transform.position + _master.transform.TransformDirection(_master.positionOffset) + Vector3.up * yOffset + Vector3.up * _stateMachine.yLag;
            
            Vector3 initialTargetPosition = actorPosition + direction * (distance + _stateMachine.zLag);
            targetPosition = HandleCameraCollision(actorPosition, initialTargetPosition, distance);
            
            return actorPosition;
        }

        private Vector3 HandleCameraCollision(Vector3 actorPosition, Vector3 targetPosition, float originalDistance)
        {
            Vector3 cameraDirection = (targetPosition - actorPosition).normalized;
            float actualDistance = originalDistance * _boostDistance;

            if (Physics.SphereCast(actorPosition, _master.collisionRadius, cameraDirection, out RaycastHit hit, actualDistance, _master.collisionMask))
            {
                float adjustedDistance = Mathf.Max(hit.distance - _master.collisionRadius, 0.1f);
                return actorPosition + cameraDirection * adjustedDistance;
            }

            return targetPosition;
        }

        protected virtual void LookAxis()
        {
            if (_isAuto)
            {
                AutoLookDirection();
            }
            else
            {
                _sensSpeedMod = 1f;
                _stateMachine.xAutoLook = 0;
            }
            
            Vector2 v = _actor.input.lookVector * (_master.sensitivity * _sensSpeedMod);
            _stateMachine.x += v.x + _stateMachine.xAutoLook;
            _stateMachine.y -= v.y;
            _stateMachine.y = Mathf.Clamp(_stateMachine.y, -75, 85);
        }

        private void ZLag()
        {
            Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
            Vector3 localVel = _actor.transform.InverseTransformDirection(vel);
            float zLag = Mathf.Clamp(localVel.z * 0.075f, 0, _master.zLagMax);
            _stateMachine.zLag = Mathf.SmoothDamp(_stateMachine.zLag, zLag, ref _stateMachine.zLagVelocity, _master.zLagTime);
        }

        private void YLag()
        {
            Vector3 vel = _actor.kinematics.Velocity;
            float yLag = Mathf.Clamp(vel.y * -0.15f, _master.yLagMin, _master.yLagMax); // min is down lag, max value is up lag
            _stateMachine.yLag = Mathf.SmoothDamp(_stateMachine.yLag, yLag, ref _stateMachine.yLagVelocity, _master.yLagTime, 10f);

            float mod = _actor.kinematics.HorizontalSpeed * 0.0175f;
            
            // Look Y Time values
            float up = 0.6f;
            float down = 0.5f;
            float restore = 0.75f;

            yLag *= yLag > 0 ? 1.3f : 0.3f;
            
            float value = yLag < 0 ? up : Mathf.Approximately(yLag, 0) ? restore - restore * mod : down;
            _lookYTime = Mathf.Lerp(_lookYTime, value, Time.deltaTime * 4f);
            _master.lookOffset.y = Mathf.SmoothDamp(_master.lookOffset.y, -yLag * 1.25f, ref _lookVelocity, _lookYTime);
        }

        protected virtual void AutoLookDirection()
        {
            float speed = _actor.kinematics.Velocity.magnitude;
            BaseActorConfig config = _actor.config;
            float lookMod = speed / config.topSpeed;
            _sensSpeedMod = Mathf.Lerp(_master.maxSensitivitySpeed, _master.minSensitivitySpeed, lookMod);

            if (speed > 0.1f)
            {
                Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
                _velocity = Vector3.Lerp(_velocity, vel, Time.deltaTime * 8f);
                float yAutoLook = Mathf.Abs(_velocity.y) > 0.2f ? Mathf.Clamp(-_velocity.y * 7.5f, _master.verticalMinAmplitude, _master.verticalMaxAmplitude)
                    : _master.verticalDefaultAmplitude;
                
                _stateMachine.yAutoLook = yAutoLook;
                _stateMachine.y = Mathf.SmoothDamp(_stateMachine.y, _stateMachine.yAutoLook, ref _yAutoLookVelocity, 0.65f);
                AutoLook(_master.horizontalAutoLookAmplitude * Mathf.Max(_master.horizontalAutoLookMinAmplitude, Mathf.Clamp01(lookMod)));
            }
            else
            {
                _stateMachine.xAutoLook = 0;
                _stateMachine.yAutoLook = 0;
            }
        }

        protected void AutoLook(float multiplier)
        {
            float fwd = _actor.stats.GetForwardSignedAngle() * Time.deltaTime;
            float dot = Vector3.Dot(Vector3.Cross(_stateMachine.transform.right, Vector3.up), _actor.transform.forward);

            if (!Mathf.Approximately(dot, -1))
            {
                _stateMachine.xAutoLook = fwd * multiplier;
            }                        
        }

        private void Setup(Vector3 targetPosition, Vector3 actorPosition)
        {
            SetPosition(targetPosition);
            SetRotation(actorPosition);
        }

        protected virtual void SetPosition(Vector3 targetPosition)
        {
            _stateMachine.position = targetPosition;
        }

        protected virtual void SetRotation(Vector3 actorPosition)
        {
            Vector3 lookTarget = actorPosition + _master.lookOffset;
            Vector3 lookDirection = lookTarget - _stateMachine.position;
            
            _stateMachine.rotation = Quaternion.LookRotation(lookDirection.normalized);
        }

        public void SetDirection(Vector3 transformForward)
        {
            Quaternion direction = Quaternion.LookRotation(transformForward, Vector3.up);
            _stateMachine.x = direction.eulerAngles.y;
            _stateMachine.y = direction.eulerAngles.x;
        }

        public void SetBoostDistance(float value)
        {
            _boostDistance = value;
        }
        
        public float GetBoostDistance()
        {
            return _boostDistance;
        }

        private bool NotOnTheWall()
        {
            return !(1 - Mathf.Abs(Vector3.Dot(_actor.transform.forward, Vector3.up)) < 0.01f);
        }
    }
}
