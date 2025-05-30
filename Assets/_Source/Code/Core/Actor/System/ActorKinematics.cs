using System;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.System
{
    /// <summary>
    /// Base actor class for a movement physics.
    /// </summary>
    public class ActorKinematics : ActorComponent
    {
        public Rigidbody Rigidbody => _rigidbody;
        [SerializeField, Range(25, 90)] public float maxAngleDifference = 80;
        public KinematicsMode mode = KinematicsMode.Free;

        [Header("Snap Normal")] 
        [SerializeField] private float normalLerpSpeed = 7f;
        [SerializeField] private float normalSpeedThreshold = 10f;
        
        public event Action<KinematicsMode> OnModeChange;

        public float TurnRate
        {
            get => _turnRate;
            set => _turnRate = value;
        }

        public float Speed => _speed;
        public float HorizontalSpeed
        {
            get
            {
                Vector3 vel = _rigidbody.linearVelocity;
                Vector3 projected = Vector3.ProjectOnPlane(vel, Normal);
                return projected.magnitude;
            }
        }

        public float Angle => _angle;
        public Vector3 Point { get; set; }
        public Vector3 Normal { get; set; }
        public Vector3 Velocity => _rigidbody.linearVelocity;
        
        public Vector3 MovementVector
        {
            get => _movementVector;
            set => _movementVector = value;
        }
        
        public Vector3 PlanarVelocity => _planarVelocity;

        public bool Skidding => _skidding;
        public float MoveDot => _moveDot;

        private Vector3 _inputDir;
        private Rigidbody _rigidbody;
        private Transform _cameraTransform;
        private Vector3 _movementVector;
        private Vector3 _planarVelocity;

        private SplineData _splineData;

        private float _speed;
        private float _moveDot;
        private float _turnRate;
        private float _angle;
        private float _detachTimer;
        private bool _canAttach;
        private bool _skidding;

        private BaseActorConfig _config;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            Normal = Vector3.up;

            _config = Actor.config;
        }

        private void Update()
        {
            CalculateInputDirection();
            CalculateMovementStats();
            CalculateDetachState();
        }

        private void FixedUpdate()
        {
            SplineCalculation();
        }

        private void CalculateInputDirection()
        {
            _cameraTransform = Actor.Camera.GetCameraTransform();

            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, Normal) *
                                       (_cameraTransform.rotation * Actor.Input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, Normal);
            _inputDir = transformedInput.normalized * Actor.Input.moveVector.magnitude;
        }

        private void CalculateMovementStats()
        {
            _moveDot = Vector3.Dot(Actor.Kinematics.GetInputDir().normalized, _rigidbody.linearVelocity.normalized);
            _skidding = _moveDot < _config.skiddingThreshold;
            _speed = _rigidbody.linearVelocity.magnitude;
            _angle = Vector3.Angle(Normal, Vector3.up);
        }

        public void BasePhysics(Vector3 normal, MovementType movementType = MovementType.Ground)
        {
            Vector3 vel = _rigidbody.linearVelocity;
            Vector3 dir = _inputDir;
            SurgeMath.SplitPlanarVector(vel, normal, out Vector3 planar, out Vector3 vertical);
            
            WriteMovementVector(planar);
            _planarVelocity = planar;

            if (movementType == MovementType.Ground)
            {
                if (_inputDir.magnitude > 0.2f)
                {
                    if (!_skidding)
                    {
                        _turnRate = Mathf.Lerp(_turnRate, _config.turnSpeed, _config.turnSmoothing * Time.fixedDeltaTime);
                        float accelRateMod = _config.accelerationCurve.Evaluate(_planarVelocity.magnitude / _config.topSpeed);
                        if (_planarVelocity.magnitude < _config.topSpeed)
                            _planarVelocity += dir * (_config.accelerationRate * accelRateMod * Time.fixedDeltaTime);
                        else if (CanReturnToBaseSpeed())
                            _planarVelocity = Vector3.MoveTowards(_planarVelocity, _planarVelocity.normalized * _config.topSpeed, 8f * Time.fixedDeltaTime);
                        
                        BaseGroundPhysics();
                    }
                    else
                    {
                        Deceleration(_config.minSkiddingRate, _config.maxSkiddingRate);
                    }
                }
                else
                {
                    Deceleration(_config.minDecelerationRate, _config.maxDecelerationRate);
                }
            }
            else
            {
                if (_inputDir.magnitude > 0.2f)
                {
                    if (!_skidding)
                    {
                        _turnRate = Mathf.Lerp(_turnRate, _config.turnSpeed, _config.turnSmoothing * Time.fixedDeltaTime);
                        float accelRateMod = _config.accelerationCurve.Evaluate(_planarVelocity.magnitude / _config.topSpeed);
                        if (_planarVelocity.magnitude < _config.topSpeed)
                            _planarVelocity += dir * (_config.accelerationRate * accelRateMod * Time.fixedDeltaTime);
                        
                        BaseAirPhysics();
                    }
                    else
                    {
                        Deceleration(_config.airDecelerationRate, _config.airDecelerationRate);
                    }
                }
            }
            
            _rigidbody.linearVelocity = _movementVector + vertical;
        }

        private void SplineCalculation()
        {
            if (_splineData != null && mode == KinematicsMode.Forward || mode == KinematicsMode.Side)
            {
                _splineData.EvaluateWorld(out var pos, out var tg, out var up, out var right);
                Project(right);
                
                _inputDir = Vector3.ProjectOnPlane(_inputDir, right);

                Vector3 endPos = pos;
                endPos += up;
                endPos.y = _rigidbody.position.y;
                
                _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, endPos, Mathf.Min(Speed / 64f, 1) * 16f * Time.fixedDeltaTime);
                _splineData.Time += Vector3.Dot(Velocity, Vector3.ProjectOnPlane(tg, Normal)) * Time.fixedDeltaTime;

                float distance = Vector3.Distance(_rigidbody.position, pos);
                if (distance > 5f)
                {
                    Debug.Log("Too far away from the point. Resetting path.");
                    SetPath(null);
                }
            }
        }

        private void BaseGroundPhysics()
        {
            Vector3 newVelocity = Quaternion.FromToRotation(_planarVelocity.normalized, _inputDir.normalized) * _planarVelocity;
            float handling = _turnRate;
            handling *= _config.turnCurve.Evaluate(_planarVelocity.magnitude / _config.topSpeed);
            _movementVector = Vector3.Slerp(_planarVelocity, newVelocity, handling * Time.fixedDeltaTime);
            
            Project();
        }

        private void BaseAirPhysics()
        {
            float handling = _turnRate;
            handling *= _config.airControl;
            _movementVector = Vector3.Lerp(_planarVelocity, _inputDir.normalized * _planarVelocity.magnitude, 
                handling * Time.fixedDeltaTime);
        }

        public void SlopePhysics()
        {
            SlopePrediction();
            
            if (_speed < _config.slopeMinSpeed && _angle >= _config.slopeDeslopeAngle)
            {
                _rigidbody.AddForce(Normal * _config.slopeDeslopeForce, ForceMode.Impulse);
                Actor.stateMachine.SetState<FStateAir>(_config.slopeInactiveDuration);
            }
            
            if (_angle > _config.slopeMinAngle && _speed > _config.slopeMinForceSpeed)
            {
                bool uphill = Vector3.Dot(_rigidbody.linearVelocity.normalized, Vector3.down) < 0;
                Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, Normal) * (1 * (uphill ? _config.slopeUphillForce : _config.slopeDownhillForce));
                _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            
            float rDot = Vector3.Dot(Vector3.up, Actor.transform.right);
            if (Mathf.Abs(rDot) > 0.1f && Mathf.Approximately(_angle, 90))
            {
                _rigidbody.linearVelocity += Vector3.down * (4 * Time.fixedDeltaTime);
            }
        }

        #region BIG CODE

        private void SlopePrediction()
        {
            float lowerValue = 0.43f;
            Vector3 predictedPosition = _rigidbody.position + -Normal * lowerValue;
            Vector3 predictedNormal = Normal;
            Vector3 predictedVelocity = _rigidbody.linearVelocity;
            float speedFrame = _rigidbody.linearVelocity.magnitude * Time.fixedDeltaTime;
            float lerpJump = 0.015f;
            LayerMask mask = _config.castLayer;
            
            if (!Physics.Raycast(predictedPosition, predictedVelocity.normalized, 
                    out RaycastHit pGround, speedFrame * 1.3f, mask)) { HighSpeedFix(); return; }

            for (float lerp = lerpJump; lerp < maxAngleDifference / 90; lerp += lerpJump)
            {
                if (!Physics.Raycast(predictedPosition, Vector3.Lerp(predictedVelocity.normalized, Normal, lerp), out pGround, speedFrame * 1.3f, mask))
                {
                    lerp += lerpJump;
                    Physics.Raycast(predictedPosition + Vector3.Lerp(predictedVelocity.normalized, Normal, lerp) * (speedFrame * 1.3f), -predictedNormal, 
                        out pGround, _config.castDistance + 0.2f, mask);

                    predictedPosition = predictedPosition + Vector3.Lerp(predictedVelocity.normalized, Normal, lerp) * speedFrame + pGround.normal * lowerValue;
                    predictedVelocity = Quaternion.FromToRotation(Normal, pGround.normal) * predictedVelocity;
                    Normal = pGround.normal;
                    _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, predictedPosition, Time.fixedDeltaTime);
                    _rigidbody.linearVelocity = predictedVelocity;
                    break;
                }
            }
        }

        private void HighSpeedFix()
        {
            Vector3 predictedPosition = _rigidbody.position;
            Vector3 predictedNormal = Actor.Stats.groundNormal;
            Vector3 predictedVelocity = _rigidbody.linearVelocity;
            int steps = 16;
            LayerMask mask = _config.castLayer;
            int i;
            for (i = 0; i < steps; i++)
            {
                predictedPosition += predictedVelocity * Time.fixedDeltaTime / steps;
                if (Physics.Raycast(predictedPosition, -predictedNormal, out RaycastHit pGround, _config.castDistance + 0.2f, mask))
                {
                    if (Vector3.Angle(predictedNormal, pGround.normal) < 45)
                    {
                        predictedPosition = pGround.point + pGround.normal * 0.5f;
                        predictedVelocity = Quaternion.FromToRotation(Actor.Stats.groundNormal, pGround.normal) * predictedVelocity;
                        predictedNormal = pGround.normal;
                    } 
                    else
                    {
                        i = -1;
                        break;
                    }
                } 
                else
                {
                    i = -1;
                    break;
                }
            }
            if (i >= steps)
            {
                Actor.Stats.groundNormal = predictedNormal;
                _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, predictedPosition, Time.fixedDeltaTime);
            }
        }

        public bool CheckForPredictedGround(Vector3 velocity, Vector3 normal, float deltaTime, float distance, int steps)
        {
            bool willBeGrounded = false;
            Vector3 initialVelocity = velocity;
            Vector3 predictedNormal = normal;
            Vector3 predictedPos = _rigidbody.position;
            for (int i = 0; i < steps; i++)
            {
                predictedPos += velocity * deltaTime / steps;
                if (Physics.Raycast(predictedPos, -predictedNormal, out RaycastHit hit, distance, _config.castLayer))
                {
                    if (Vector3.Angle(predictedNormal, hit.normal) < maxAngleDifference)
                    {
                        predictedPos = hit.point + hit.normal;
                        predictedNormal = hit.normal;
                        initialVelocity = Quaternion.FromToRotation(Normal, predictedNormal) * initialVelocity;
                        willBeGrounded = true;
                    }
                }
            }

            return willBeGrounded;
        }

        #endregion

        public void ResetVelocity()
        {
            if (Rigidbody.isKinematic) 
                return;
            
            Rigidbody.linearVelocity = Vector3.zero;
        }
        
        public void ApplyGravity(float yGravity)
        {
            if (!Rigidbody.isKinematic) Rigidbody.linearVelocity += Vector3.down * (yGravity * Time.fixedDeltaTime);
        }
        
        public bool CheckForGround(out RaycastHit result, CheckGroundType type = CheckGroundType.Normal, float castDistance = 0f)
        {
            Vector3 origin;
            Vector3 direction;
            switch (type)
            {
                case CheckGroundType.Normal:
                    origin = Actor.transform.position;
                    direction = -Actor.Kinematics.Normal;
                    break;
                case CheckGroundType.DefaultDown:
                    origin = Actor.transform.position;
                    direction = -Actor.transform.up;
                    break;
                case CheckGroundType.Predict:
                    origin = Actor.transform.position;
                    direction = Actor.Kinematics.Rigidbody.linearVelocity.normalized;
                    break;
                case CheckGroundType.PredictJump:
                    origin = Actor.transform.position - Actor.transform.up * 0.5f;
                    direction = Actor.Kinematics.Rigidbody.linearVelocity.normalized;
                    break;
                case CheckGroundType.PredictOnRail:
                    origin = Actor.transform.position + Actor.transform.forward;
                    direction = -Actor.Kinematics.Normal;
                    break;
                case CheckGroundType.PredictEdge:
                    origin = Actor.transform.position + Vector3.ClampMagnitude(PlanarVelocity * 0.075f, 1f);
                    direction = -Actor.Kinematics.Normal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            Ray ray = new Ray(origin, direction);
            if (castDistance == 0) castDistance = _config.castDistance;
            LayerMask castMask = Actor.config.castLayer;

            bool hit = Physics.Raycast(ray, out result,
                castDistance, castMask, QueryTriggerInteraction.Ignore);
            
            return hit;
        }
        
        public bool CheckForGroundWithDirection(out RaycastHit result, Vector3 direction,
            float castDistance = 0f)
        {
            Vector3 origin = Actor.transform.position;
            
            if (castDistance == 0) castDistance = Actor.config.castDistance;
            
            Ray ray = new Ray(origin, direction);
            LayerMask castMask = Actor.config.castLayer;
            bool hit = Physics.Raycast(ray, out result, castDistance, castMask, QueryTriggerInteraction.Ignore);
            return hit;
        }
        
        public bool CheckForCeiling(out RaycastHit result)
        {
            bool hit = Physics.Raycast(Actor.transform.position - Actor.transform.up * 0.5f, Actor.transform.up, out result, Actor.config.castDistance * 0.5f, Actor.config.castLayer, QueryTriggerInteraction.Ignore);
            return hit;
        }
        
        public void Project(Vector3 normal = default)
        {
            if (normal == default)
            {
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, Normal);
                return;
            }
            
            _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
        }

        public void Snap(Vector3 point, Vector3 normal, bool instant = false)
        {
            if (!_canAttach) return;
            
            Vector3 goal = point + normal;
            if (instant) _rigidbody.position = point + normal;
            else
            {
                Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, normal) * _rigidbody.rotation;
                _rigidbody.position = Vector3.Lerp(_rigidbody.position, goal, Time.fixedDeltaTime * (Mathf.Abs(Quaternion.Dot(_rigidbody.rotation, slopeRotation) + 1f) / 2f * _rigidbody.linearVelocity.magnitude + 10f));
            }
        }

        public void SlerpSnapNormal(Vector3 targetNormal)
        {
            if (Speed > normalSpeedThreshold)
            {
                Normal = Vector3.Slerp(Normal, targetNormal, (normalLerpSpeed + Speed / 2) * Time.fixedDeltaTime);
            }
            else
            {
                Normal = Vector3.Slerp(Normal, Vector3.up, normalLerpSpeed * Time.fixedDeltaTime);
            }
        }

        public void Deceleration(float min, float max)
        {
            if (!CanDecelerate())
                return;
            
            float f = Mathf.Lerp(max, min, 
                _movementVector.magnitude / _config.topSpeed);
            if (_movementVector.magnitude > 0.02f)
                _movementVector = Vector3.MoveTowards(_movementVector, Vector3.zero, Time.fixedDeltaTime * f);
            else
            {
                _movementVector = Vector3.zero;
                SetStateOnZeroSpeed(Actor.stateMachine.CurrentState);
            }
        }

        protected virtual void SetStateOnZeroSpeed(FState state)
        {
            switch (state)
            {
                case FStateAir:
                    break;
                default:
                    Actor.stateMachine.SetState<FStateIdle>();
                    break;
            }
        }

        protected virtual bool CanReturnToBaseSpeed()
        {
            return false;
        }

        protected virtual bool CanDecelerate()
        {
            return !Actor.Flags.HasFlag(FlagType.OutOfControl);
        }
        
        public void SetDetachTime(float t)
        {
            _detachTimer = 0;
            _canAttach = false;
            
            _detachTimer = t;
        }

        public void SetInputDir(Vector3 dir)
        {
            _inputDir = dir;
        }

        public void WriteMovementVector(Vector3 vector)
        {
            _movementVector = vector;
        }

        private void CalculateDetachState()
        {
            if (_detachTimer > 0f)
            {
                _canAttach = false;
                
                _detachTimer -= Time.deltaTime;
            }
            else
            {
                _canAttach = true;

                _detachTimer = 0f;
            }
        }

        public bool GetAttachState() => _canAttach;
        
        public Vector3 GetInputDir()
        {
            return _inputDir;
        }
        
        public void SetPath(SplineContainer path, KinematicsMode desiredMode = KinematicsMode.Free)
        {
            Vector3 pos = Rigidbody.position - Rigidbody.transform.up * 0.5f;
            _splineData = path != null ? new SplineData(path, pos) : null;
            
            mode = desiredMode;
            if (path == null)
            {
                mode = KinematicsMode.Free;
            }
            
            OnModeChange?.Invoke(mode);
        }
        
        public bool IsPathValid()
        {
            return _splineData != null;
        }

        public SplineContainer GetPath()
        {
            return _splineData.Container;
        }
    }

    public class SplineData
    {
        public float Time { get; set; }
        public float Length { get; private set; }
        public float NormalizedTime => Time / Length;
        public SplineContainer Container => _container;

        private readonly SplineContainer _container;

        public SplineData(SplineContainer container, Vector3 position)
        {
            _container = container;
            
            SplineUtility.GetNearestPoint(container.Spline, _container.transform.InverseTransformPoint(position), 
                out _, out var f, 8, 3);
            
            Length = container.Spline.GetLength();
            Time = f * Length;
        }

        public void Evaluate(out Vector3 position, out Vector3 tangent, out Vector3 up, out Vector3 right)
        {
            var spline = _container.Spline;

            spline.Evaluate(NormalizedTime, 
                out var pos,
                out var tg,
                out var upVector);
            
            position = pos;
            tangent = ((Vector3)tg).normalized;
            up = upVector;
            right = Vector3.Cross(tg, Vector3.up).normalized;
        }

        public void EvaluateWorld(out Vector3 position, out Vector3 tangent, out Vector3 up, out Vector3 right)
        {
            var transform = _container.transform;

            float normalizedTime = Mathf.Clamp01(Mathf.Max(0.001f, Time / Length));
            var spline = _container.Spline;

            spline.Evaluate(normalizedTime, 
                out var pos,
                out var tg,
                out var upVector);
            
            position = transform.TransformPoint(pos);
            tangent = transform.TransformDirection(tg).normalized;
            up = transform.TransformDirection(upVector);
            right = Vector3.Cross(tangent, -up).normalized;
        }
    }

    public struct SplineSample
    {
        public Vector3 pos;
        public Vector3 tg;
        public Vector3 up;
        public Vector3 right;
        
        public Vector3 ProjectOnUp(Vector3 vector) => Vector3.ProjectOnPlane(vector, Vector3.up);
    }

    public enum KinematicsMode
    {
        Free,
        Forward,
        Dash,
        Side // 2D
    }

    public enum MovementType
    {
        Ground,
        Air
    }
}