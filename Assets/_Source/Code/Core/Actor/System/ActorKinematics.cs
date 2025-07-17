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

        [Header("Gravity")] 
        [SerializeField] private float initialGravity;
        public float Gravity { get; set; }
        public float InitialGravity => initialGravity;
        public float AirTime { get; private set; }
        
        [Header("Prediction")]
        [SerializeField, Range(25, 90)] public float maxAngleDifference = 80;
        public KinematicsMode mode = KinematicsMode.Free;

        [Header("Normal")] 
        [SerializeField] private float normalLerpSpeed = 7f;
        [SerializeField] private float normalSpeedThreshold = 10f;
        [SerializeField, Range(0, 90), Tooltip("When slope angle is greater than this value, the player will slide down")] private float hardAngle = 60f;
        
        public event Action<KinematicsMode> OnModeChange;

        public float Speed => _rigidbody.linearVelocity.magnitude;
        public float HorizontalSpeed
        {
            get
            {
                Vector3 vel = _rigidbody.linearVelocity;
                Vector3 projected = Vector3.ProjectOnPlane(vel, Normal);
                return projected.magnitude;
            }
        }

        public Vector3 Point { get; set; }
        public Vector3 Normal { get; set; }
        public float Angle => Vector3.Angle(Normal, Vector3.up);
        public Vector3 Velocity => _rigidbody.linearVelocity;
        public Vector3 MovementVector => _movementVector;
        public Vector3 PlanarVelocity => _planarVelocity;
        public Vector3 Vertical => _vertical;
        public float TurnRate { get; set; }
        public bool Skidding => _moveDot < _config.skiddingThreshold;
        public float MoveDot => _moveDot;

        private Vector3 _inputDir;
        private Rigidbody _rigidbody;
        private Transform _cameraTransform;
        private Vector3 _movementVector;
        private Vector3 _planarVelocity;
        private Vector3 _vertical;
        private Vector3 _normalSmoothVelocity;

        private SplineData _splineData;
        private Vector3 _lastTangent;
        
        private float _moveDot;
        private float _detachTimer;
        private bool _canAttach;

        private PhysicsConfig _config;
        private float _maxSpeedMultiplier = 1f;

        private void Awake()
        {
            _rigidbody = Actor.Rigidbody;
            _config = Actor.Config;
            _maxSpeedMultiplier = 1f;
            
            if (initialGravity == 0) initialGravity = Mathf.Abs(Physics.gravity.y);
            Gravity = initialGravity;

            Normal = Vector3.up;
        }

        protected virtual void Update()
        {
            CalculateInputDirection();
            CalculateMovementStats();
            CalculateDetachState();
            CheckIfIsInAir();
        }

        protected virtual void FixedUpdate()
        {
            SplineCalculation();
        }

        private void CalculateInputDirection()
        {
            _cameraTransform = Actor.Camera.GetCameraTransform();

            if (!Actor.Flags.HasFlag(FlagType.Autorun))
            {
                Vector3 rawInput = _cameraTransform.rotation * Actor.Input.moveVector;
                Vector3 orientedInput = Quaternion.FromToRotation(_cameraTransform.up, Normal) * rawInput;
                _inputDir = GetMovementDirectionProjectedOnPlane(orientedInput, Normal, _cameraTransform.up)
                            * Actor.Input.moveVector.magnitude;
            }
        }

        private void CalculateMovementStats()
        {
            _moveDot = Vector3.Dot(Actor.Kinematics.GetInputDir().normalized, Velocity.normalized);
        }

        private void CheckIfIsInAir()
        {
            if (InAir())
            {
                AirTime += Time.deltaTime;
            }
            else
            {
                AirTime = 0;
            }
        }

        public void BasePhysics(Vector3 normal, MovementType movementType = MovementType.Ground)
        {
            Vector3 vel = _rigidbody.linearVelocity;
            Vector3 dir = _inputDir;
            SurgeMath.SplitPlanarVector(vel, normal, out Vector3 planar, out var vertical);
            //vertical = Vector3.ClampMagnitude(vertical, _config.maxVerticalSpeed);
            
            WriteMovementVector(planar);
            _planarVelocity = planar;

            if (movementType == MovementType.Ground)
            {
                if (_inputDir.magnitude > 0.2f)
                {
                    if (!Skidding)
                    {
                        TurnRate = Mathf.Lerp(TurnRate, _config.turnSpeed, _config.turnSmoothing * Time.fixedDeltaTime);
                        float accelRateMod = _config.accelerationCurve.Evaluate(_planarVelocity.magnitude / _config.topSpeed);
                        if (_planarVelocity.magnitude < _config.topSpeed)
                            _planarVelocity += dir * (_config.accelerationRate * accelRateMod * Time.fixedDeltaTime);
                        else if (CanReturnToBaseSpeed())
                            _planarVelocity = Vector3.MoveTowards(_planarVelocity, _planarVelocity.normalized * _config.topSpeed, 6f * Time.fixedDeltaTime);
                        
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
                    if (!Skidding)
                    {
                        TurnRate = Mathf.Lerp(TurnRate, _config.turnSpeed, _config.turnSmoothing * Time.fixedDeltaTime);
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
            
            _rigidbody.linearVelocity = _movementVector;

            if (movementType == MovementType.Air)
            {
                _rigidbody.linearVelocity += vertical;
            }
        }

        private void SplineCalculation()
        {
            if (_splineData != null)
            {
                if (mode == KinematicsMode.Forward || mode == KinematicsMode.Side)
                {
                    _splineData.EvaluateWorld(out var pos, out var tg, out var up, out var right);

                    if (_lastTangent != Vector3.zero)
                    {
                        Rigidbody.linearVelocity = Quaternion.FromToRotation(_lastTangent, tg) * Rigidbody.linearVelocity;
                    }

                    _lastTangent = tg;

                    if (right != Vector3.zero)
                    {
                        _inputDir = Vector3.ProjectOnPlane(_inputDir, right);
                        _inputDir = Vector3.ProjectOnPlane(_inputDir, tg);
                        Project(right);
                    }
                    
                    Vector3 endPos = pos;
                    endPos += up;
                    endPos.y = _rigidbody.position.y;
                
                    _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, endPos, Mathf.Min(Speed / 64f, 1) * 16f * Time.fixedDeltaTime);
                    _splineData.Time += Vector3.Dot(Velocity, Vector3.ProjectOnPlane(tg, up)) * Time.fixedDeltaTime;

                    float distance = Vector3.Distance(_rigidbody.position, pos);
                    if (distance > 5f)
                    {
                        Debug.Log("Too far away from the point. Resetting path.");
                        
                        var flags = Actor.Flags;
                        if (flags.HasFlag(FlagType.Autorun))
                        {
                            flags.RemoveFlag(FlagType.Autorun);
                        }
                        
                        SetPath(null);
                    }
                }
            }
            else
            {
                SetPath(null);
            }
        }

        private void BaseGroundPhysics()
        {
            Vector3 newVelocity = Quaternion.FromToRotation(_planarVelocity.normalized, Vector3.ProjectOnPlane(_inputDir.normalized, Vector3.up)) * _planarVelocity;
            float handling = TurnRate;
            handling *= _config.turnCurve.Evaluate(Speed / _config.topSpeed);
            _movementVector = Vector3.Slerp(_planarVelocity, newVelocity, handling * Time.fixedDeltaTime);
        }

        private void BaseAirPhysics()
        {
            float handling = TurnRate;
            handling *= _config.airControl;
            _movementVector = Vector3.Lerp(_planarVelocity, _inputDir.normalized * _planarVelocity.magnitude, 
                handling * Time.fixedDeltaTime);
        }

        public void SlopePhysics()
        {
            if (mode == KinematicsMode.Free && Speed < _config.slopeMinSpeed && Angle >= _config.slopeDeslopeAngle)
            {
                _rigidbody.AddForce(Normal * _config.slopeDeslopeForce, ForceMode.Impulse);
                Actor.StateMachine.SetState<FStateAir>();
                SetDetachTime(_config.slopeInactiveDuration);
            }

            if (Angle > _config.slopeMinAngle && Speed > _config.slopeMinForceSpeed)
            {
                bool uphill = Vector3.Dot(_rigidbody.linearVelocity.normalized, Vector3.down) < 0;
                float forceMag = uphill ? _config.slopeUphillForce : _config.slopeDownhillForce;
                Vector3 slopeDir = GetMovementDirectionProjectedOnPlane(Vector3.down, Normal, Vector3.up);
                _rigidbody.AddForce(slopeDir * (forceMag * Time.fixedDeltaTime), ForceMode.Impulse);
            }

            float rDot = Vector3.Dot(Vector3.up, Actor.transform.right);
            if (Mathf.Abs(rDot) > 0.1f && Mathf.Approximately(Angle, 90f))
                _rigidbody.linearVelocity += Vector3.down * (4f * Time.fixedDeltaTime);
        }

        private Vector3 GetMovementDirectionProjectedOnPlane(Vector3 movement, Vector3 groundNormal, Vector3 upDirection)
        {
            Vector3 movementProjectedOnPlane = Vector3.ProjectOnPlane(movement, groundNormal);
            Vector3 axisToRotateAround = Vector3.Cross(movement, upDirection);
            float angle = Vector3.SignedAngle(movement, movementProjectedOnPlane, axisToRotateAround);
            Quaternion rotation = Quaternion.AngleAxis(angle, axisToRotateAround);
            return (rotation * movement).normalized;
        }

        public bool CheckForPredictedGround(float deltaTime, float distance, int steps)
        {
            bool willBeGrounded = false;
            Vector3 velocity = Velocity;
            Vector3 initialVelocity = velocity;
            Vector3 normal = Normal;
            Vector3 predictedNormal = normal;
            Vector3 predictedPos = _rigidbody.position;
            for (int i = 0; i < steps; i++)
            {
                predictedPos += velocity * deltaTime / steps;
                Ray ray = new Ray(predictedPos, -predictedNormal);
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.blue);
                if (Physics.Raycast(ray, out RaycastHit hit, distance, _config.castLayer))
                {
                    var angle = Vector3.Angle(predictedNormal, hit.normal);
                    if (angle > maxAngleDifference)
                    {
                        predictedPos = hit.point + hit.normal;
                        predictedNormal = hit.normal;
                        initialVelocity = Quaternion.FromToRotation(normal, predictedNormal) * initialVelocity;
                        willBeGrounded = true;
                    }
                }
            }

            return willBeGrounded;
        }

        public void ResetVelocity()
        {
            if (Rigidbody.isKinematic) 
                return;
            
            Rigidbody.linearVelocity = Vector3.zero;
        }
        
        public void ApplyGravity(float yGravity)
        {
            if (!Rigidbody.isKinematic)
            {
                Rigidbody.linearVelocity += Vector3.down * (yGravity * Time.fixedDeltaTime);
            }
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
            LayerMask castMask = Actor.Config.castLayer;

            bool hit = Physics.Raycast(ray, out result,
                castDistance, castMask, QueryTriggerInteraction.Ignore);
            
            return hit;
        }
        
        public bool CheckForGroundWithDirection(out RaycastHit result, Vector3 direction,
            float castDistance = 0f)
        {
            Vector3 origin = Actor.transform.position;
            
            if (castDistance == 0) castDistance = Actor.Config.castDistance;
            
            Ray ray = new Ray(origin, direction);
            LayerMask castMask = Actor.Config.castLayer;
            bool hit = Physics.Raycast(ray, out result, castDistance, castMask, QueryTriggerInteraction.Ignore);
            return hit;
        }
        
        public bool CheckForCeiling(out RaycastHit result)
        {
            bool hit = Physics.Raycast(Actor.transform.position - Actor.transform.up * 0.5f, Actor.transform.up, out result, Actor.Config.castDistance * 0.5f, Actor.Config.castLayer, QueryTriggerInteraction.Ignore);
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

        public void ClampVelocityToMax()
        {
            var flags = Actor.Flags;
            if (!flags.HasFlag(FlagType.OutOfControl) && !flags.HasFlag(FlagType.Autorun))
            {
                Rigidbody.linearVelocity = Vector3.ClampMagnitude(Rigidbody.linearVelocity, _config.maxSpeed);
            }
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

        public void RotateSnapNormal(Vector3 targetNormal)
        {
            float minSpeed = normalSpeedThreshold * 0.5f;
            float maxSpeed = normalSpeedThreshold * 1.5f;
            float t = Mathf.Clamp01((Speed - minSpeed) / (maxSpeed - minSpeed));
    
            Vector3 goal = Vector3.Slerp(Vector3.up, targetNormal, t);
            float smoothTime = 1f / (normalLerpSpeed + Speed * t);
            Normal = Vector3
                .SmoothDamp(Normal, goal, ref _normalSmoothVelocity, smoothTime, Mathf.Infinity, Time.fixedDeltaTime)
                .normalized;
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
                SetStateOnZeroSpeed(Actor.StateMachine.CurrentState);
            }
        }

        protected virtual void SetStateOnZeroSpeed(FState state)
        {
            switch (state)
            {
                case FStateAir:
                    break;
                default:
                    Actor.StateMachine.SetState<FStateIdle>();
                    break;
            }
        }

        protected virtual bool CanReturnToBaseSpeed()
        {
            return false;
        }

        protected virtual bool CanDecelerate()
        {
            var flags = Actor.Flags;
            bool canDecelerate = !flags.HasFlag(FlagType.OutOfControl) && !flags.HasFlag(FlagType.Autorun);
            return canDecelerate;
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

        public bool IsHardAngle(Vector3 normal) => Vector3.Angle(normal, Vector3.up) > hardAngle;

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

        public virtual bool InAir() => Actor.StateMachine.CurrentState is FStateAir;
        
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

        public SplineData GetPath()
        {
            return _splineData;
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

        public Vector3 EvaluatePosition()
        {
            EvaluateWorld(out var pos, out _, out _, out _);
            return pos;
        }
        
        public Vector3 EvaluateTangent()
        {
            EvaluateWorld(out _, out var tg, out _, out _);
            return tg;
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