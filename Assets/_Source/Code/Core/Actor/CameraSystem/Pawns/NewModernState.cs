using SurgeEngine.Code.Core.Actor.CameraSystem.Modifiers;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns
{
    public class NewModernState : CameraState<PanData>
    {
        private float _sensSpeedMod;
        private Vector3 _velocity;
        private float _lookVelocity;
        private float _currentCollisionDistance;
        private float _lookYTime;
        private float _yAutoLookVelocity;
        private float _xAutoLookVelocity;

        private const float RisingSmoothingTime = 0.6f;
        private const float FallingSmoothingTime = 0.5f;
        private const float RestoreSmoothingTime = 0.75f;
        private const float SpeedModFactor = 0.0175f;
        private const float LerpSpeed = 5f;
        private const float YLagMultiplier = 0.75f;
        private const float YLagRisingFactor = 1.3f;
        private const float YLagFallingFactor = 0.3f;
        private const float ZLagFactor = 0.075f;
        private const float YLagVelocityFactor = -0.125f;
        private const float MinSpeedThreshold = 0.1f;
        private const float VelocityLerpSpeed = 8f;
        private const float YAutoLookSmoothTime = 0.25f;
        private const float AutoLookResetSpeed = 12f;
        private const float DefaultSensitivityResetSpeed = 6f;
        private const float MinPitch = -75f;
        private const float MaxPitch = 85f;
        private const float LateralOffsetLerpSpeed = 1.6f;
        private const float LateralOffsetResetSpeed = 3.25f;
        private const float MinCollisionDistance = 0.1f;

        private bool IsAuto => _actor.input.IsAutoCamera();

        public NewModernState(ActorBase owner) : base(owner)
        {
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            LookAxis();
            ModernSetup();
        }

        private void ModernSetup()
        {
            float distance = 1;
            if (_stateMachine.master.GetModifier(out BoostDistanceCameraModifier boostDistance))
            {
                distance = boostDistance.Value;
            }

            float fov = 1;
            if (_stateMachine.master.GetModifier(out BoostViewCameraModifier boostView))
            {
                fov = boostView.Value;
            }

            _stateMachine.fov = _stateMachine.baseFov * fov;
            
            Vector3 actorPosition = CalculateTarget(out Vector3 targetPosition, _stateMachine.actualDirection, _stateMachine.distance * distance);
            ZLag();
            YLag();
            LateralOffset();
            Setup(targetPosition, actorPosition);
        }

        protected Vector3 CalculateTarget(out Vector3 targetPosition, Vector3 dir, float distance)
        {
            Vector3 actorPosition = _stateMachine.actorPosition;
            Vector3 initialTargetPosition = actorPosition + dir * (distance + _stateMachine.zLag);
            targetPosition = HandleCameraCollision(actorPosition, initialTargetPosition, distance);
            return actorPosition;
        }

        private Vector3 HandleCameraCollision(Vector3 actorPosition, Vector3 targetPosition, float originalDistance)
        {
            Vector3 cameraDirection = (targetPosition - actorPosition).normalized;
            
            float distance = 1;
            if (_stateMachine.master.GetModifier(out BoostDistanceCameraModifier boostDistance))
            {
                distance = boostDistance.Value;
            }
            float actualDistance = originalDistance * distance;

            if (Physics.SphereCast(actorPosition, _master.collisionRadius, cameraDirection, out RaycastHit hit, actualDistance, _master.collisionMask))
            {
                float adjustedDistance = Mathf.Max(hit.distance - _master.collisionRadius, MinCollisionDistance);
                return actorPosition + cameraDirection * adjustedDistance;
            }
            return targetPosition;
        }

        protected virtual void LookAxis()
        {
            if (IsAuto && !_stateMachine.is2D)
            {
                AutoLookDirection();
            }
            else
            {
                _sensSpeedMod = 1f;
                _stateMachine.xAutoLook = Mathf.Lerp(_stateMachine.xAutoLook, 0, Time.deltaTime * DefaultSensitivityResetSpeed);
            }

            Vector2 lookInput = _actor.input.lookVector * (_master.sensitivity * _sensSpeedMod);
            _stateMachine.x += lookInput.x + _stateMachine.xAutoLook;
            _stateMachine.y = Mathf.Clamp(_stateMachine.y - lookInput.y, MinPitch, MaxPitch);
        }

        private void ZLag()
        {
            Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
            Vector3 localVel = _actor.transform.InverseTransformDirection(vel);
            float zLag = Mathf.Clamp(localVel.z * ZLagFactor, 0, _master.zLagMax);
            _stateMachine.zLag = Mathf.SmoothDamp(_stateMachine.zLag, zLag, ref _stateMachine.zLagVelocity, _master.zLagTime);
        }

        private void YLag()
        {
            Vector3 vel = _actor.kinematics.Velocity;
            float targetYLag = _actor.stateMachine.CurrentState is FStateAir or FStateSpecialJump 
                ? Mathf.Clamp(vel.y * YLagVelocityFactor, _master.yLagMin, _master.yLagMax) 
                : 0f;
            _stateMachine.yLag = Mathf.SmoothDamp(_stateMachine.yLag, targetYLag, ref _stateMachine.yLagVelocity, _master.yLagTime);

            float adjustedYLag = targetYLag * (targetYLag > 0 ? YLagRisingFactor : YLagFallingFactor);
            float targetLookYTime = adjustedYLag < 0 ? RisingSmoothingTime 
                : adjustedYLag > 0 ? FallingSmoothingTime 
                : RestoreSmoothingTime * (1 - _actor.kinematics.Speed * SpeedModFactor);

            _lookYTime = Mathf.Lerp(_lookYTime, targetLookYTime, Time.deltaTime * LerpSpeed);
            _master.lookOffset.y = Mathf.SmoothDamp(_master.lookOffset.y, -adjustedYLag * YLagMultiplier, ref _lookVelocity, _lookYTime);
        }

        protected virtual void AutoLookDirection()
        {
            float speed = _actor.kinematics.Speed;
            float lookMod = speed / _actor.config.topSpeed;
            _sensSpeedMod = Mathf.Lerp(_master.maxSensitivitySpeed, _master.minSensitivitySpeed, lookMod);

            if (speed > MinSpeedThreshold)
            {
                Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
                _velocity = Vector3.Lerp(_velocity, vel, Time.deltaTime * VelocityLerpSpeed);
                float yAutoLook = Mathf.Clamp(-vel.y, _master.verticalMinAmplitude, _master.verticalMaxAmplitude);
                _stateMachine.yAutoLook = yAutoLook + _master.verticalDefaultAmplitude;
                _stateMachine.y = Mathf.SmoothDamp(_stateMachine.y, _stateMachine.yAutoLook, ref _yAutoLookVelocity, YAutoLookSmoothTime);

                float multiplier = _master.horizontalAutoLookAmplitude * Mathf.Max(_master.horizontalAutoLookMinAmplitude, Mathf.Clamp01(lookMod));
                AutoLook(multiplier);
            }
            else
            {
                _stateMachine.xAutoLook = Mathf.Lerp(_stateMachine.xAutoLook, 0, Time.deltaTime * AutoLookResetSpeed);
                _stateMachine.yAutoLook = Mathf.Lerp(_stateMachine.yAutoLook, 0, Time.deltaTime * AutoLookResetSpeed);
            }
        }

        protected virtual void AutoLook(float multiplier)
        {
            float angle = GetAutoAngle() * (1 - _stateMachine.sideBlendFactor) * Time.deltaTime;
            float dot = Vector3.Dot(Vector3.Cross(_stateMachine.transform.right, Vector3.up), _actor.transform.forward);
            if (!Mathf.Approximately(dot, -1))
            {
                _stateMachine.xAutoLook = angle * multiplier;
            }
        }

        protected void LateralOffset()
        {
            float x = _actor.input.moveVector.x;
            float time = _actor.kinematics.Speed / _actor.config.topSpeed;
            AnimationCurve curve = _master.LateralOffsetSpeedCurve;
            float modifier = 0;

            if (_stateMachine.master.GetModifier(out DriftCameraModifier driftCameraModifier))
            {
                modifier += driftCameraModifier.Value;
            }
            
            if (Mathf.Abs(x) > 0)
            {
                float viewDot = Vector3.Dot(_stateMachine.transform.forward, -_master.Actor.transform.up);
                viewDot = Mathf.Abs(viewDot);

                if (viewDot >= 0.9f)
                {
                    _master.lookOffset.x = Mathf.Lerp(_master.lookOffset.x, 0, Time.deltaTime * LateralOffsetResetSpeed * 4);
                }
                else
                {
                    _master.lookOffset.x = Mathf.Lerp(_master.lookOffset.x, x * 0.6f * curve.Evaluate(time) * modifier, Time.deltaTime * LateralOffsetLerpSpeed);
                }
            }
            else
            {
                _master.lookOffset.x = Mathf.Lerp(_master.lookOffset.x, 0, Time.deltaTime * LateralOffsetResetSpeed);
            }
        }
        
        private void Setup(Vector3 targetPosition, Vector3 actorPosition)
        {
            SetPosition(targetPosition);
            SetRotation(actorPosition);
        }

        protected virtual void SetPosition(Vector3 targetPosition) => _stateMachine.position = targetPosition;

        protected virtual void SetRotation(Vector3 actorPosition)
        {
            Vector3 lookTarget = actorPosition + _stateMachine.transform.TransformDirection(new Vector3(_master.lookOffset.x, 0, 0));
            Vector3 lookDirection = lookTarget - _stateMachine.position;
            _stateMachine.rotation = Quaternion.LookRotation(lookDirection.normalized);
        }

        public float GetAutoAngle()
        {
            Vector3 crossedForward = Vector3.Cross(_actor.transform.right, Vector3.up);
            Vector3 crossedCamForward = Vector3.Cross(_stateMachine.transform.right, Vector3.up);
            Vector3 forward = Vector3.ProjectOnPlane(crossedForward, Vector3.up).normalized;
            Vector3 camForward = Vector3.ProjectOnPlane(crossedCamForward, Vector3.up).normalized;
            return Vector3.SignedAngle(forward, camForward, -Vector3.up);
        }
    }
}