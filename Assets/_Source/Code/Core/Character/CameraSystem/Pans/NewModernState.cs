using System;
using System.Linq;
using SurgeEngine._Source.Code.Core.Character.CameraSystem.Modifiers;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans
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
        private const float LateralOffsetResetSpeed = 3.25f;
        private const float MinCollisionDistance = 0.1f;

        private bool IsAuto => Character.Input.IsAutoCamera();

        public NewModernState(CharacterBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _pitchAuto = 0;
            _yawAuto = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            LookAxis();
            
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

            StateFOV = _stateMachine.BaseFov * fov;
            
            ZLag();
            YLag(_master.YLagMin, _master.YLagMax);
            LateralOffset();
            
            Setup(CalculateCameraTarget(GetDistance() * distance), CalculateTarget());
        }

        private Vector3 CalculateCameraTarget(float originalDistance)
        {
            var horizontal = Quaternion.AngleAxis(_stateMachine.Yaw, Vector3.up);
            var vertical = Quaternion.AngleAxis(_stateMachine.Pitch, Vector3.right);
            Vector3 dir = horizontal * vertical * Vector3.back;

            var pos = CalculateTarget();
            
            float maxDist = originalDistance * 
                (_stateMachine.Master.GetModifier<BoostDistanceCameraModifier>(out var m) ? m.Value : 1f) + _forwardLag;
            float collDist = CalculateCollisionDistance(pos, StatePosition - pos, maxDist);
            return pos + dir * collDist;
        }

        protected virtual Vector3 CalculateTarget()
        {
            return Character.transform.position + Vector3.up * GetVerticalOffset() + Vector3.up * _verticalLag;
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

            Vector2 lookInput = Character.Input.LookVector * (_master.Sensitivity * _master.UserSensitivityMultiplier);
            _stateMachine.Yaw += lookInput.x + _yawAuto;
            _stateMachine.Pitch = Mathf.Clamp(_stateMachine.Pitch - lookInput.y, MinPitch, MaxPitch);
        }

        protected virtual float GetDistance() => _master.Distance;
        protected virtual float GetVerticalOffset() => _master.YOffset;

        private void ZLag()
        {
            Vector3 vel = Character.Kinematics.Rigidbody.linearVelocity;
            Vector3 localVel = Character.transform.InverseTransformDirection(vel);
            float zLag = Mathf.Clamp(localVel.z * ZLagFactor, 0, _master.ZLagMax);
            _forwardLag = Mathf.SmoothDamp(_forwardLag, zLag, ref _forwardLagVelocity, _master.ZLagTime);
        }

        protected virtual void YLag(float min, float max)
        {
            Type[] excludedStates = new [] { typeof(FStateAfterHoming), typeof(FStateGrind), typeof(FStateGrindSquat) };
            bool isExcludedState =
                excludedStates.Any(state => state.IsAssignableFrom(Character.StateMachine.CurrentState.GetType()));
            
            Vector3 vel = Character.Kinematics.Velocity;
            bool allowLag = !Character.Kinematics.CheckForGround(out _) && !isExcludedState; // In the air
            float targetYLag = allowLag
                ? Mathf.Clamp(vel.y * YLagVelocityFactor, min, max) 
                : 0f;
            
            float progression = Mathf.Clamp01(1 - Mathf.Abs(vel.y) / Character.Config.topSpeed);
            targetYLag = Mathf.Lerp(_verticalLag, targetYLag, progression);
            _verticalLag = Mathf.SmoothDamp(_verticalLag, targetYLag, ref _verticalLagVelocity, _master.YLagTime);

            float adjustedYLag = targetYLag * (targetYLag > 0 ? YLagRisingFactor : YLagFallingFactor);
            float targetLookYTime = adjustedYLag < 0 ? RisingSmoothingTime 
                : adjustedYLag > 0 ? FallingSmoothingTime 
                : RestoreSmoothingTime * (1 - Character.Kinematics.Speed * SpeedModFactor);

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
            float speed = Character.Kinematics.Speed;
            float lookMod = speed / Character.Config.topSpeed;
            
            if (speed > MinSpeedThreshold)
            {
                Vector3 vel = Character.Kinematics.Velocity;
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
            float dot = Vector3.Dot(Vector3.Cross(_stateMachine.Transform.right, Vector3.up), Character.transform.forward);
            if (dot > -0.95f)
            {
                _yawAuto = angle * multiplier * _stateMachine.BlendFactor;
            }
        }

        private void LateralOffset()
        {
            float x = Character.Input.MoveVector.x;
            float time = Character.Kinematics.Speed / Character.Config.topSpeed;
            AnimationCurve curve = _master.LateralOffsetSpeedCurve;
            float modifier = 0;

            if (_stateMachine.Master.GetModifier(out DriftCameraModifier driftCameraModifier))
            {
                modifier += driftCameraModifier.Value;
            }
            
            if (Mathf.Abs(x) > 0)
            {
                float viewDot = Vector3.Dot(_stateMachine.Transform.forward, -_master.character.transform.up);
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
        
        /// <summary>
        /// Setup the camera state.
        /// </summary>
        /// <param name="targetPosition">Camera goal position</param>
        /// <param name="characterPosition">Camera goal rotation target</param>
        private void Setup(Vector3 targetPosition, Vector3 characterPosition)
        {
            SetPosition(targetPosition);
            SetRotation(characterPosition);
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
            Vector3 crossedForward = Vector3.Cross(Character.transform.right, Vector3.up);
            Vector3 crossedCamForward = Vector3.Cross(_stateMachine.Transform.right, Vector3.up);
            Vector3 forward = Vector3.ProjectOnPlane(crossedForward, Vector3.up).normalized;
            Vector3 camForward = Vector3.ProjectOnPlane(crossedCamForward, Vector3.up).normalized;
            return Vector3.SignedAngle(forward, camForward, -Vector3.up);
        }

        private Vector3 GetOffset()
        {
            float yOffset = _lookOffset.y;
            
            Vector3 vertical = Vector3.up * yOffset;
            Vector3 horizontal = new Vector3(_lookOffset.x, 0, 0);
            
            return vertical + _stateMachine.Transform.TransformDirection(horizontal);
        }
    }
}