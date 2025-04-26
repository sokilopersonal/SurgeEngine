using System;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Tools;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Actor.System
{
    public class ActorKinematics : ActorComponent
    {
        public Rigidbody Rigidbody => _rigidbody;
        [SerializeField, Range(25, 90)] public float maxAngleDifference = 75;
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
        
        public Vector3 PlanarVelocity
        {
            get => _planarVelocity;
            set => _planarVelocity = value;
        }
        
        public bool Skidding => _skidding;
        public float MoveDot => _moveDot;

        private Vector3 _inputDir;
        private Rigidbody _rigidbody;
        private Transform _cameraTransform;
        private Vector3 _movementVector;
        private Vector3 _planarVelocity;

        private SplineContainer _path;
        private SplineContainer _rail;
        private Vector3 _prevTg;

        private float _speed;
        private float _moveDot;
        private float _turnRate;
        private float _angle;
        private float _detachTimer;
        private bool _canAttach;
        private bool _skidding;

        private BaseActorConfig _config;

        public void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            Normal = Vector3.up;

            _config = Actor.config;
        }

        private void Update()
        {
            _cameraTransform = Actor.camera.GetCameraTransform();
            
            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, Normal) *
                                       (_cameraTransform.rotation * Actor.input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, Normal);
            _inputDir = transformedInput.normalized * Actor.input.moveVector.magnitude;
            
            _moveDot = Vector3.Dot(Actor.kinematics.GetInputDir().normalized, _rigidbody.linearVelocity.normalized);
            
            _skidding = _moveDot < _config.skiddingThreshold;
            _speed = _rigidbody.linearVelocity.magnitude;
            
            CalculateDetachState();
            
            _angle = Vector3.Angle(Normal, Vector3.up);
        }

        private void FixedUpdate()
        {
            SplineCalculation();
        }

        public void BasePhysics(Vector3 point, Vector3 normal, MovementType movementType = MovementType.Ground)
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
                        else if (!SonicTools.IsBoost())
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
            Snap(point, normal, true);
        }
        
        public void SplineCalculation()
        {
            // TODO: Move all spline data to a data class
            if (_path != null && mode == KinematicsMode.Forward || mode == KinematicsMode.Side)
            {
                var spline = _path.Spline;
                Vector3 localPos = _path.transform.InverseTransformPoint(_rigidbody.position);
                SplineUtility.GetNearestPoint(spline, localPos, out var p, out var f, 12, 8);
                f *= spline.GetLength();

                SplineSample sample = new SplineSample
                {
                    pos = _path.EvaluatePosition(f / spline.GetLength()),
                    tg = ((Vector3)_path.EvaluateTangent(f / spline.GetLength())).normalized,
                    up = _path.EvaluateUpVector(f / spline.GetLength())
                };
                
                if (_prevTg == Vector3.zero) _prevTg = sample.tg;
                
                Vector3 splinePlane = Vector3.Cross(sample.tg, sample.up);
                Vector3 upSplinePlane = Vector3.Cross(sample.tg, Vector3.up);
                
                _inputDir = Vector3.ProjectOnPlane(_inputDir, splinePlane);
                
                _rigidbody.linearVelocity = Quaternion.FromToRotation(Vector3.ProjectOnPlane(_prevTg, Vector3.up).normalized, 
                    Vector3.ProjectOnPlane(sample.tg, Vector3.up)) * _rigidbody.linearVelocity;
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, upSplinePlane);
                _prevTg = sample.tg;

                Vector3 newPos = sample.pos;
                
                SurgeMath.SplitPlanarVector(_rigidbody.position, 
                    sample.ProjectOnUp(sample.tg).normalized, 
                    out var pLat, 
                    out var pVer);
                
                SurgeMath.SplitPlanarVector(newPos, sample.ProjectOnUp(sample.tg).normalized, 
                    out var sLat,
                    out var sVer);

                var targetY = _rigidbody.position.y;
                
                pLat = Vector3.MoveTowards(pLat, sLat, Mathf.Min(Speed / 64f, 1) * 16f * Time.fixedDeltaTime);
                pLat.y = targetY;
                
                _rigidbody.position = pLat + pVer;
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
            Vector3 predictedNormal = Actor.stats.groundNormal;
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
                        predictedVelocity = Quaternion.FromToRotation(Actor.stats.groundNormal, pGround.normal) * predictedVelocity;
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
                Actor.stats.groundNormal = predictedNormal;
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

        public void Project(Vector3 normal = default)
        {
            if (normal == default)
            {
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, Normal);
                return;
            }
            
            _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
        }

        private void BaseAirPhysics()
        {
            float handling = _turnRate;
            handling *= _config.airControl;
            _movementVector = Vector3.Lerp(_planarVelocity, _inputDir.normalized * _planarVelocity.magnitude, 
                handling * Time.fixedDeltaTime);
        }

        public void Snap(Vector3 point, Vector3 normal, bool instant = false)
        {
            if (!_canAttach) return;
            
            Vector3 goal = point + normal;
            
            if (point != Vector3.zero && normal != Vector3.zero)
            {
                if (instant) _rigidbody.position = point + normal;
                else
                {
                    Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, normal) * _rigidbody.rotation;
                    _rigidbody.position = Vector3.Lerp(_rigidbody.position, goal, Time.fixedDeltaTime * (Mathf.Abs(Quaternion.Dot(_rigidbody.rotation, slopeRotation) + 1f) / 2f * _rigidbody.linearVelocity.magnitude + 10f));
                }
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
            if (SonicTools.IsBoost()) return;
            if (Actor.flags.HasFlag(FlagType.OutOfControl)) return;
            
            float f = Mathf.Lerp(max, min, 
                _movementVector.magnitude / _config.topSpeed);
            if (_movementVector.magnitude > 0.02f)
                _movementVector = Vector3.MoveTowards(_movementVector, Vector3.zero, Time.fixedDeltaTime * f);
            else
            {
                _movementVector = Vector3.zero;
                switch (Actor.stateMachine.CurrentState)
                {
                    case FStateAir:
                        break;
                    case FStateCrawl:
                        break;
                    case FStateSweepKick:
                        break;
                    case FStateSit:
                        break;
                    default:
                        Actor.stateMachine.SetState<FStateIdle>();
                        break;
                }
            }
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

        public void ModifyTurnRate(float modifier) => _turnRate *= modifier;
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
            mode = desiredMode;
            _path = path;
            OnModeChange?.Invoke(mode);

            if (path == null)
            {
                mode = KinematicsMode.Free;
            }
        }
        
        public bool IsPathValid()
        {
            return _path != null;
        }

        public SplineContainer GetPath()
        {
            return _path;
        }
    }
    
    public struct SplineSample
    {
        public Vector3 pos;
        public Vector3 tg;
        public Vector3 up;
        
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