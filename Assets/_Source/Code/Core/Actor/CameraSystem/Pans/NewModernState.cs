using System;
using System.Linq;
using SurgeEngine.Code.Core.Actor.CameraSystem.Modifiers;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class NewModernState : CameraState
    {
        private float _lookVelocity;
        private float _currentCollisionDistance;
        private float _lookYTime;
        private float _xAutoLookVelocity;
        private float _yAutoLookVelocity;
        
        private float _verticalLag;
        private float _verticalLagVelocity;
        private float _forwardLag;
        private float _forwardLagVelocity;

        private Vector3 _lookOffset;
        private float _yawAuto;
        private float _pitchAuto;
        
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
        private const float YAutoLookSmoothTime = 0.25f;
        private const float AutoLookResetSpeed = 12f;
        private const float MinPitch = -75f;
        private const float MaxPitch = 85f;
        private const float LateralOffsetLerpSpeed = 1.6f;
        private const float LateralOffsetResetSpeed = 3.25f;
        private const float MinCollisionDistance = 0.1f;

        private bool IsAuto => _actor.Input.IsAutoCamera();

        public NewModernState(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            StateFOV = 55f;
            
            ModernSetup();
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
            if (_stateMachine.Master.GetModifier(out BoostDistanceCameraModifier boostDistance))
            {
                distance = boostDistance.Value;
            }

            float fov = 1;
            if (_stateMachine.Master.GetModifier(out BoostViewCameraModifier boostView))
            {
                fov = boostView.Value;
            }

            StateFOV = 55f * fov;
            
            var horizontal = Quaternion.AngleAxis(_stateMachine.Yaw, Vector3.up);
            var vertical = Quaternion.AngleAxis(_stateMachine.Pitch, Vector3.right);
            Vector3 dir = horizontal * vertical * Vector3.back;
            
            Vector3 actorPosition = CalculateTarget(out Vector3 targetPosition, dir, GetDistance() * distance);
            ZLag();
            YLag(_master.YLagMin, _master.YLagMax);
            LateralOffset();
            Setup(targetPosition, actorPosition);
        }

        protected Vector3 CalculateTarget(out Vector3 targetPosition, Vector3 dir, float originalDistance)
        {
            Vector3 actorPos = _actor.transform.position 
                               + Vector3.up * GetVerticalOffset() 
                               + Vector3.up * _verticalLag;
            
            float maxDist = originalDistance * (_stateMachine.Master.GetModifier<BoostDistanceCameraModifier>(out var m) ? m.Value : 1f);
            float collDist = CalculateCollisionDistance(actorPos, StatePosition - actorPos, maxDist);
            targetPosition = actorPos + dir * (collDist + _forwardLag);
            return actorPos;
        }

        protected virtual float CalculateCollisionDistance(Vector3 origin, Vector3 direction, float baseDistance)
        {
            if (Physics.SphereCast(origin, _master.CollisionRadius, direction, out var hit, baseDistance, _master.CollisionMask))
                return Mathf.Max(hit.distance, MinCollisionDistance);
            return baseDistance;
        }

        protected virtual void LookAxis()
        {
            if (IsAuto)
            {
                AutoLookDirection();
            }
            else
            {
                _yawAuto =  0;
            }

            Vector2 lookInput = _actor.Input.lookVector * _master.Sensitivity;
            _stateMachine.Yaw += lookInput.x + _yawAuto;
            _stateMachine.Pitch = Mathf.Clamp(_stateMachine.Pitch - lookInput.y, MinPitch, MaxPitch);
        }

        protected virtual float GetDistance() => _master.Distance;
        protected virtual float GetVerticalOffset() => _master.YOffset;

        private void ZLag()
        {
            Vector3 vel = _actor.Kinematics.Rigidbody.linearVelocity;
            Vector3 localVel = _actor.transform.InverseTransformDirection(vel);
            float zLag = Mathf.Clamp(localVel.z * ZLagFactor, 0, _master.ZLagMax);
            _forwardLag = Mathf.SmoothDamp(_forwardLag, zLag, ref _forwardLagVelocity, _master.ZLagTime);
        }

        protected virtual void YLag(float min, float max)
        {
            Type[] excludedStates = new [] { typeof(FStateAfterHoming), typeof(FStateGrind), typeof(FStateGrindSquat) };
            bool isExcludedState =
                excludedStates.Any(state => state.IsAssignableFrom(_actor.StateMachine.CurrentState.GetType()));
            
            Vector3 vel = _actor.Kinematics.Velocity;
            bool allowLag = !_actor.Kinematics.CheckForGround(out _) && !isExcludedState; // In the air
            float targetYLag = allowLag
                ? Mathf.Clamp(vel.y * YLagVelocityFactor, min, max) 
                : 0f;
            
            float progression = Mathf.Clamp01(1 - Mathf.Abs(vel.y) / _actor.Config.topSpeed);
            targetYLag = Mathf.Lerp(_verticalLag, targetYLag, progression);
            _verticalLag = Mathf.SmoothDamp(_verticalLag, targetYLag, ref _verticalLagVelocity, _master.YLagTime);

            float adjustedYLag = targetYLag * (targetYLag > 0 ? YLagRisingFactor : YLagFallingFactor);
            float targetLookYTime = adjustedYLag < 0 ? RisingSmoothingTime 
                : adjustedYLag > 0 ? FallingSmoothingTime 
                : RestoreSmoothingTime * (1 - _actor.Kinematics.Speed * SpeedModFactor);

            _lookYTime = Mathf.Lerp(_lookYTime, targetLookYTime, Time.deltaTime * LerpSpeed);
            if (allowLag)
            {
                _lookOffset.y = Mathf.SmoothDamp(_lookOffset.y, -adjustedYLag * YLagMultiplier, ref _lookVelocity, _lookYTime);
            }
            else
            {
                _lookOffset.y = Mathf.SmoothDamp(_lookOffset.y, 0, ref _lookVelocity, 0.5f);
            }
        }

        private void AutoLookDirection()
        {
            float speed = _actor.Kinematics.Speed;
            float lookMod = speed / _actor.Config.topSpeed;
            
            if (speed > MinSpeedThreshold)
            {
                Vector3 vel = _actor.Kinematics.Velocity;
                float yAutoLook = Mathf.Clamp(-vel.y, _master.YawMinAmplitude, _master.YawMaxAmplitude);
                _pitchAuto = yAutoLook + _master.YawDefaultAmplitude;
                _stateMachine.Pitch = Mathf.SmoothDamp(_stateMachine.Pitch, _pitchAuto, ref _yAutoLookVelocity, YAutoLookSmoothTime);

                float multiplier = _master.PitchAutoLookAmplitude * Mathf.Max(_master.PitchAutoLookMinAmplitude, Mathf.Clamp01(lookMod));
                AutoLook(multiplier);
            }
            else
            {
                _yawAuto = Mathf.Lerp(_yawAuto, 0, Time.deltaTime * AutoLookResetSpeed);
                _pitchAuto = Mathf.Lerp(_pitchAuto, 0, Time.deltaTime * AutoLookResetSpeed);
            }
        }

        private void AutoLook(float multiplier)
        {
            float angle = GetAutoAngle() * Time.deltaTime;
            float dot = Vector3.Dot(_actor.transform.forward, Vector3.up);
            if (1 - Mathf.Abs(dot) > 0.05f)
            {
                _yawAuto = angle * multiplier;
            }
        }

        private void LateralOffset()
        {
            float x = _actor.Input.moveVector.x;
            float time = _actor.Kinematics.Speed / _actor.Config.topSpeed;
            AnimationCurve curve = _master.LateralOffsetSpeedCurve;
            float modifier = 0;

            if (_stateMachine.Master.GetModifier(out DriftCameraModifier driftCameraModifier))
            {
                modifier += driftCameraModifier.Value;
            }
            
            if (Mathf.Abs(x) > 0)
            {
                float viewDot = Vector3.Dot(_stateMachine.Transform.forward, -_master.Actor.transform.up);
                viewDot = Mathf.Abs(viewDot);

                if (viewDot >= 0.9f)
                {
                    _lookOffset.x = Mathf.Lerp(_lookOffset.x, 0, Time.deltaTime * LateralOffsetResetSpeed * 4);
                }
                else
                {
                    _lookOffset.x = Mathf.Lerp(_lookOffset.x, x * 0.6f * curve.Evaluate(time) * modifier, Time.deltaTime * 1.25f);
                }
            }
            else
            {
                _lookOffset.x = Mathf.Lerp(_lookOffset.x, 0, Time.deltaTime * LateralOffsetResetSpeed);
            }
        }
        
        private void Setup(Vector3 targetPosition, Vector3 actorPosition)
        {
            SetPosition(targetPosition);
            SetRotation(actorPosition);
        }

        protected virtual void SetPosition(Vector3 targetPosition)
        {
            StatePosition = targetPosition;
        }

        protected virtual void SetRotation(Vector3 actorPosition)
        {
            StateRotation = Quaternion.LookRotation(actorPosition + GetOffset() - StatePosition);
        }

        private float GetAutoAngle()
        {
            Vector3 forward = Vector3.ProjectOnPlane(_actor.Rigidbody.transform.forward, Vector3.up);
            Vector3 camForward = Vector3.ProjectOnPlane(_stateMachine.Transform.forward, Vector3.up);
            float angle = Vector3.SignedAngle(forward, camForward, -Vector3.up);
            return angle;
        }

        protected Vector3 GetOffset()
        {
            float yOffset = _lookOffset.y;
            
            Vector3 vertical = Vector3.up * yOffset;
            Vector3 horizontal = new Vector3(_lookOffset.x, 0, 0);
            
            return vertical + _stateMachine.Transform.TransformDirection(horizontal);
        }
    }
}