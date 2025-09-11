using System;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Gameplay.CommonObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.ChangeModes;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Infrastructure.Config;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.System
{
    /// <summary>
    /// Base actor class for a movement physics.
    /// </summary>
    public class CharacterKinematics : CharacterComponent, IPointMarkerLoader
    {
        public Rigidbody Rigidbody { get; private set; }

        [Header("Physics")]
        [SerializeField] private float baseSpeedRestorationDelta = 16f;

        [Header("Gravity")] 
        [SerializeField] private float initialGravity;
        public float Gravity { get; set; }
        public float InitialGravity => initialGravity;
        public float AirTime { get; private set; }
        
        [Header("Prediction")]
        [SerializeField, Range(25, 90)] public float maxAngleDifference = 80;

        [Header("Normal")] 
        [SerializeField] private float normalLerpSpeed = 7f;
        [SerializeField] private float normalSpeedThreshold = 10f;

        public float Speed => Velocity.magnitude;

        public Vector3 Point { get; set; }
        public Vector3 Normal { get; set; }
        public ReactiveVar<GroundTag> GroundTag { get; private set; } = new();
        public float Angle => Vector3.Angle(Normal, Vector3.up);
        public Vector3 Velocity
        {
            get
            {
                if (!Rigidbody.isKinematic)
                    return Rigidbody.linearVelocity;

                return _kinematicVelocity;
            }
        }
        public Vector3 HorizontalVelocity => Vector3.ProjectOnPlane(Velocity, Rigidbody.transform.up);
        public Vector3 VerticalVelocity => Vector3.Project(Velocity, Rigidbody.transform.up);
        public Vector3 PlanarVelocity => _planarVelocity;
        public float TurnRate { get; set; }
        public bool Skidding => _moveDot < _config.skiddingThreshold;
        public float MoveDot => _moveDot;
        
        private Vector3 _inputDir;
        private Transform _cameraTransform;
        private Vector3 _movementVector;
        private Vector3 _planarVelocity;
        private Vector3 _kinematicVelocity;
        private Vector3 _lastPosition;
        private Vector3 _waterSnapVelocity;

        public ChangeMode2DData Path2D { get; private set; }
        public ChangeMode3DData PathForward { get; private set; }
        public ChangeMode3DData PathDash { get; private set; }

        private float _moveDot;
        private float _detachTimer;
        private bool _canAttach;

        private PhysicsConfig _config;

        private void Awake()
        {
            Rigidbody = character.Rigidbody;
            Rigidbody.sleepThreshold = -1;

            _config = character.Config;

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
            CalculatePath2D();
            CalculatePathForward();
            CalculatePathDash();

            Vector3 position = Rigidbody.position;
            _kinematicVelocity = (position - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = position;
        }

        private void CalculateInputDirection()
        {
            _cameraTransform = character.Camera.GetCameraTransform();

            if (!character.Flags.HasFlag(FlagType.Autorun))
            {
                Vector3 rawInput = _cameraTransform.rotation * character.Input.moveVector;
                Vector3 orientedInput = Quaternion.FromToRotation(_cameraTransform.up, Normal) * rawInput;
                _inputDir = SurgeMath.GetMovementDirectionProjectedOnPlane(orientedInput, Normal, _cameraTransform.up)
                            * character.Input.moveVector.magnitude;
            }
            
            Debug.DrawRay(transform.position, _inputDir, Color.red, 0, false);
        }

        private void CalculateMovementStats()
        {
            _moveDot = Vector3.Dot(character.Kinematics.GetInputDir().normalized, Rigidbody.transform.forward);
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
            Vector3 vel = Rigidbody.linearVelocity;
            Vector3 dir = _inputDir;
            
            Vector3 planar = Vector3.ProjectOnPlane(vel, normal);
            Vector3 vertical = Vector3.Project(vel, normal);
            
            _movementVector = planar;
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
                            _planarVelocity = Vector3.MoveTowards(_planarVelocity, _planarVelocity.normalized * _config.topSpeed, baseSpeedRestorationDelta * Time.fixedDeltaTime);
                        
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
            
            Rigidbody.linearVelocity = _movementVector;

            if (movementType == MovementType.Air)
            {
                Rigidbody.linearVelocity += vertical;
            }
        }

        private void BaseGroundPhysics()
        {
            // why the fuck I've put the projected inputDir on vec3.up??
            Vector3 newVelocity = Quaternion.FromToRotation(_planarVelocity.normalized, _inputDir) * _planarVelocity;
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

        private void CalculatePath2D()
        {
            if (Path2D != null)
            {
                var path = Path2D.Spline;
                path.EvaluateWorld(out var pos, out var tg, out var up, out var right);
                    
                if (right != Vector3.zero)
                {
                    _inputDir = Vector3.ProjectOnPlane(_inputDir, tg);
                    Project(right);
                }

                Vector3 endPos = pos;
                endPos += up;
                endPos.y = Rigidbody.position.y;

                Vector3 target = Vector3.MoveTowards(Rigidbody.position, endPos,
                    Mathf.Min(Speed / 16f, 1) * 16f * Time.fixedDeltaTime);
                Rigidbody.MovePosition(target);
                
                if (Speed > 0.02f && character.Flags.HasFlag(FlagType.Autorun))
                {
                    float sign = Mathf.Sign(Vector3.Dot(Velocity.normalized, tg));
                    var dir = sign * tg;
                    Rigidbody.MoveRotation(Quaternion.LookRotation(dir, Normal));
                }
                        
                if (IsPathOutOfRange(Path2D))
                {
                    Path2D = null;
                }

                path.Time += Vector3.Dot(HorizontalVelocity, tg) * Time.fixedDeltaTime;
            }
        }

        private void CalculatePathForward()
        {
            if (PathForward != null)
            {
                var force = PathForward.PathCorrectionForce;
                if (force > 0)
                {
                    if (CheckForGround(out _))
                    {
                        PathForward.Spline.EvaluateWorld(out _, out var tg, out var up, out var right);
                    
                        float dot = Vector3.Dot(Velocity.normalized, tg);
                        float sign = Mathf.Sign(dot);
                        tg = sign * tg;
                    
                        var targetDir = tg.normalized * Velocity.magnitude;
                        float speedFactor = Mathf.Clamp01(1f - HorizontalVelocity.magnitude / (_config.maxSpeed * 2f));
                        float adjustedForce = force * speedFactor;

                        if (HorizontalVelocity.magnitude > 0.1f)
                        {
                            var rotation = Quaternion.RotateTowards(Quaternion.LookRotation(HorizontalVelocity), 
                                Quaternion.LookRotation(targetDir), adjustedForce * Mathf.Rad2Deg * Time.fixedDeltaTime);
                            Rigidbody.linearVelocity = rotation * Vector3.forward * HorizontalVelocity.magnitude + VerticalVelocity;
                        }
                    }
                }
                
                PathForward.Spline.Time += Vector3.Dot(Velocity, PathForward.Spline.EvaluateTangent()) * Time.fixedDeltaTime;

                if (PathForward.IsLimitEdge)
                {
                    if (IsPathOutOfRange(PathForward))
                    {
                        PathForward = null;
                    }
                }
            }
        }

        private void CalculatePathDash()
        {
            if (PathDash != null)
            {
                PathDash.Spline.Time += Vector3.Dot(Velocity, PathDash.Spline.EvaluateTangent()) * Time.fixedDeltaTime;

                if (PathDash.IsLimitEdge)
                {
                    if (IsPathOutOfRange(PathDash))
                    {
                        PathDash = null;
                    }
                }
            }
        }

        public void SlopePhysics()
        {
            if (Speed < _config.slopeMinSpeed && Angle >= _config.slopeDeslopeAngle)
            {
                Rigidbody.AddForce(Normal * _config.slopeDeslopeForce, ForceMode.Impulse);
                character.StateMachine.SetState<FStateAir>();
                SetDetachTime(_config.slopeInactiveDuration);
            }

            if (Angle > _config.slopeMinAngle && Speed > _config.slopeMinForceSpeed)
            {
                bool uphill = Vector3.Dot(Rigidbody.linearVelocity.normalized, Vector3.down) < 0;
                float forceMag = uphill ? _config.slopeUphillForce : _config.slopeDownhillForce;
                Vector3 slopeDir = SurgeMath.GetMovementDirectionProjectedOnPlane(Vector3.down, Normal, Vector3.up);
                Rigidbody.AddForce(slopeDir * (forceMag * Time.fixedDeltaTime), ForceMode.Impulse);
            }

            float rDot = Vector3.Dot(Vector3.up, character.transform.right);
            if (Mathf.Abs(rDot) > 0.1f && Mathf.Approximately(Angle, 90f))
                Rigidbody.linearVelocity += Vector3.down * (4f * Time.fixedDeltaTime);
        }

        public bool CheckForPredictedGround(float deltaTime, float distance, int steps)
        {
            bool willBeGrounded = false;
            Vector3 velocity = Velocity;
            Vector3 initialVelocity = velocity;
            Vector3 predictedNormal = Normal;
            Vector3 predictedPos = Rigidbody.position;
            for (int i = 0; i < steps; i++)
            {
                predictedPos += velocity * deltaTime / steps;
                Ray ray = new Ray(predictedPos, -predictedNormal);
                if (Physics.Raycast(ray, out RaycastHit hit, distance, _config.castLayer))
                {
                    var angle = Vector3.Angle(predictedNormal, hit.normal);
                    if (angle < maxAngleDifference)
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
                    origin = character.transform.position;
                    direction = -character.Kinematics.Normal;
                    break;
                case CheckGroundType.DefaultDown:
                    origin = character.transform.position;
                    direction = -character.transform.up;
                    break;
                case CheckGroundType.Predict:
                    origin = character.transform.position;
                    direction = character.Kinematics.Rigidbody.linearVelocity.normalized;
                    break;
                case CheckGroundType.PredictJump:
                    origin = character.transform.position - character.transform.up * 0.5f;
                    direction = character.Kinematics.Rigidbody.linearVelocity.normalized;
                    break;
                case CheckGroundType.PredictOnRail:
                    origin = character.transform.position + character.transform.forward;
                    direction = -character.Kinematics.Normal;
                    break;
                case CheckGroundType.PredictEdge:
                    origin = character.transform.position + Vector3.ClampMagnitude(PlanarVelocity * 0.075f, 1f);
                    direction = -character.Kinematics.Normal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            Ray ray = new Ray(origin, direction);
            if (castDistance == 0) castDistance = _config.castDistance;
            LayerMask castMask = character.Config.castLayer;
            
            Debug.DrawRay(origin, direction * castDistance, Color.red);
            
            bool hit = Physics.Raycast(ray, out result,
                castDistance, castMask, QueryTriggerInteraction.Collide);
            
            return hit;
        }
        
        public bool CheckForGroundWithDirection(out RaycastHit result, Vector3 direction,
            float castDistance = 0f)
        {
            Vector3 origin = character.transform.position;
            
            if (castDistance == 0) castDistance = character.Config.castDistance;
            
            Ray ray = new Ray(origin, direction);
            LayerMask castMask = character.Config.castLayer;
            bool hit = Physics.Raycast(ray, out result, castDistance, castMask, QueryTriggerInteraction.Ignore);
            return hit;
        }
        
        public bool CheckForCeiling(out RaycastHit result)
        {
            bool hit = Physics.Raycast(character.transform.position - character.transform.up * 0.5f, character.transform.up, out result, character.Config.castDistance * 0.5f, character.Config.castLayer, QueryTriggerInteraction.Ignore);
            return hit;
        }
        
        public void Project(Vector3 normal = default)
        {
            if (normal == default)
            {
                Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, Normal);
                return;
            }
            
            Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, normal);
        }

        public void ClampVelocityToMax(float max = default)
        {
            var flags = character.Flags;
            if (!flags.HasFlag(FlagType.OutOfControl) && !flags.HasFlag(FlagType.Autorun))
            {
                Rigidbody.linearVelocity = Vector3.ClampMagnitude(Rigidbody.linearVelocity, Mathf.Approximately(max, 0) ? _config.maxSpeed : max);
            }
        }

        public void Snap(Vector3 point, Vector3 normal)
        {
            if (!_canAttach) return;
            
            Vector3 goal = point + normal;
            Rigidbody.MovePosition(goal);
        }

        public void Snap(Vector3 point)
        {
            if (!_canAttach) return;
            
            Rigidbody.MovePosition(point);
        }

        public void SnapOnWater(Vector3 point)
        {
            point.y -= 0.1f;
            var end = Vector3.SmoothDamp(Rigidbody.position, point + Normal, ref _waterSnapVelocity, 0.5f);
            Snap(end);
        }

        public void RotateSnapNormal(Vector3 targetNormal)
        {
            if (Speed > normalSpeedThreshold)
            {
                float t = Speed / 2;
                Normal = Vector3.Slerp(Normal, targetNormal, t * Time.fixedDeltaTime);
            }
            else
            {
                float t = normalLerpSpeed;
                Normal = Vector3.Slerp(Normal, Vector3.up, t * Time.fixedDeltaTime);
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
                SetStateOnZeroSpeed(character.StateMachine.CurrentState);
            }
        }

        protected virtual void SetStateOnZeroSpeed(FState state)
        {
            switch (state)
            {
                case FStateAir:
                    break;
                default:
                    character.StateMachine.SetState<FStateIdle>();
                    break;
            }
        }

        protected virtual bool CanReturnToBaseSpeed()
        {
            return false;
        }

        protected virtual bool CanDecelerate()
        {
            var flags = character.Flags;
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

        public virtual bool InAir() => character.StateMachine.CurrentState is FStateAir;

        public void Set2DPath(ChangeMode2DData data)
        {
            Path2D = data;
        }

        public void SetForwardPath(ChangeMode3DData data)
        {
            PathForward = data;
        }

        public void SetDashPath(ChangeMode3DData data)
        {
            PathDash = data;
        }

        public void Load()
        {
            Set2DPath(null);
            SetForwardPath(null);
            SetDashPath(null);
        }

        private static bool IsPathOutOfRange(ChangeModeData data) => data.Spline.Time > data.Spline.Length || data.Spline.Time < 0;
    }

    public struct SplineSample
    {
        public Vector3 pos;
        public Vector3 tg;
        public Vector3 up;
        public Vector3 right;
        
        public Vector3 ProjectOnUp(Vector3 vector) => Vector3.ProjectOnPlane(vector, Vector3.up);
    }

    public enum MovementType
    {
        Ground,
        Air
    }
}