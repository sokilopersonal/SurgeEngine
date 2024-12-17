using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Tools;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorKinematics : ActorComponent
    {
        public Rigidbody Rigidbody => _rigidbody;
        
        [SerializeField, Range(25, 90)] private float maxAngleDifference = 75;

        public float TurnRate
        {
            get => _turnRate;
            set => _turnRate = value;
        }

        public float HorizontalSpeed => _rigidbody.GetHorizontalMagnitude();

        public float Angle => _angle;

        public Vector3 Normal
        {
            get => _normal;
            set => _normal = value;
        }

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
        private Vector3 _normal;
        
        private PathData _pathData;
        private SplineContainer _rail;

        private float _speed;
        private float _moveDot;
        private float _turnRate;
        private float _angle;
        private float _detachTimer;
        private float _skidTimer;
        private bool _canAttach;
        private bool _skidding;

        private BaseActorConfig _config;

        public void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            Normal = Vector3.up;

            _config = actor.config;
        }

        private void Update()
        {
            _cameraTransform = actor.camera.GetCameraTransform();
            
            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, _normal) *
                                       (_cameraTransform.rotation * actor.input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, _normal);
            _inputDir = transformedInput.normalized * actor.input.moveVector.magnitude;
            if (actor.input.moveVector == Vector3.zero && SonicTools.IsBoost()) _inputDir = _rigidbody.transform.forward;
            
            _moveDot = Vector3.Dot(actor.kinematics.GetInputDir().normalized, _rigidbody.linearVelocity.normalized);
            
            bool isSkidding = _moveDot < _config.skiddingThreshold;
            if (isSkidding)
            {
                _skidTimer += Time.deltaTime;

                if (_skidTimer > _config.skidDelay)
                {
                    _skidding = true; // To exclude the random brake
                }
            }
            else
            {
                _skidding = false;
                _skidTimer = 0f;
            }
            _speed = _rigidbody.linearVelocity.magnitude;
            
            CalculateDetachState();
            
            _angle = Vector3.Angle(_normal, Vector3.up);
        }

        private void FixedUpdate()
        {
            var path = _pathData;
            if (path != null)
            {
                if (path.splineContainer != null)
                {
                    var container = path.splineContainer;
                    SplineUtility.GetNearestPoint(container.Spline, SurgeMath.Vector3ToFloat3(container.transform.InverseTransformPoint(_rigidbody.position)), out var near, out var t);
                    container.Evaluate(t, out var point, out var tangent, out var up);
                    var planeNormal = Vector3.Cross(tangent, up);
                
                    if (HorizontalSpeed < path.maxAutoRunSpeed)
                    {
                        _rigidbody.AddForce(_rigidbody.transform.forward * (Time.fixedDeltaTime * path.autoRunSpeed), ForceMode.Impulse);
                    }
                
                    _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, planeNormal);
                    _inputDir = Vector3.ProjectOnPlane(_inputDir, planeNormal);
                    _inputDir = Vector3.ProjectOnPlane(_inputDir, up);
                
                    Vector3 nearPoint = container.transform.TransformPoint(near);
                    nearPoint.y = _rigidbody.position.y;
                    // _rigidbody.position = nearPoint;
                    // _rigidbody.rotation = Quaternion.LookRotation(tangent, up);
                }
            }
        }

        public void BasePhysics(Vector3 point, Vector3 normal)
        {
            Vector3 vel = _rigidbody.linearVelocity;
            Vector3 dir = _inputDir;
            SurgeMath.SplitPlanarVector(vel, normal, out Vector3 planar, out Vector3 vertical); 
            
            _movementVector = planar;
            _planarVelocity = planar;

            var stateMachine = actor.stateMachine;
            var state = stateMachine.CurrentState;
            
            bool isSkidding = _moveDot < _config.skiddingThreshold;
            if (_inputDir.magnitude > 0.2f)
            {
                if (!isSkidding)
                {
                    _turnRate = Mathf.Lerp(_turnRate, _config.turnSpeed, 
                        _config.turnSmoothing * Time.fixedDeltaTime);
                    
                    var accelRateMod = _config.accelerationCurve
                        .Evaluate(_planarVelocity.magnitude / _config.topSpeed);
                    if (_planarVelocity.magnitude < _config.topSpeed)
                        _planarVelocity += dir * (_config.accelerationRate * accelRateMod * Time.fixedDeltaTime);
                    else
                    {
                        if (!SonicTools.IsBoost())
                        {
                            _planarVelocity = Vector3.MoveTowards(_planarVelocity, 
                                _planarVelocity.normalized * _config.topSpeed, 8f * Time.fixedDeltaTime);
                        }
                    }

                    switch (state)
                    {
                        case FStateGround or FStateSlide:
                            BaseGroundPhysics();
                            break;
                        case FStateAir or FStateJump:
                            BaseAirPhysics();
                            break;
                    }
                }
                else
                {
                    Deceleration(_config.minSkiddingRate, _config.maxSkiddingRate);
                }
            }
            else
            {
                Deceleration(_config.minDeaccelerationRate, _config.maxDeaccelerationRate);
            }

            _rigidbody.linearVelocity = _movementVector + vertical;
            
            Snap(point, normal);
        }

        private void BaseGroundPhysics()
        {
            Vector3 newVelocity = Quaternion.FromToRotation(_planarVelocity.normalized, _inputDir.normalized) * _planarVelocity;
            float handling = _turnRate;
            handling *= _config.turnCurve.Evaluate(_planarVelocity.magnitude / _config.topSpeed);
            _movementVector = Vector3.Slerp(_planarVelocity, newVelocity, handling * Time.fixedDeltaTime);
            
            SlopePhysics();
            
            Project();
        }

        public void SlopePhysics()
        {
            SlopePrediction();
            
            if (_speed < _config.slopeMinSpeed && _angle >= _config.slopeDeslopeAngle)
            {
                _rigidbody.AddForce(_normal * _config.slopeDeslopeForce, ForceMode.Impulse);
                actor.stateMachine.SetState<FStateAir>(_config.slopeInactiveDuration);
            }
            
            if (_angle > _config.slopeMinAngle && _speed > _config.slopeMinForceSpeed)
            {
                bool uphill = Vector3.Dot(_rigidbody.linearVelocity.normalized, Vector3.down) < 0;
                Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, _normal) * (1 * (uphill ? _config.slopeUphillForce : _config.slopeDownhillForce));
                _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            
            float rDot = Vector3.Dot(Vector3.up, actor.transform.right);
            if (Mathf.Abs(rDot) > 0.1f && Mathf.Approximately(_angle, 90))
            {
                _rigidbody.linearVelocity += Vector3.down * (4 * Time.fixedDeltaTime);
            }
        }

        #region BIG CODE

        public void SlopePrediction()
        {
            var lowerValue = 0.45f;
            var predictedPosition = _rigidbody.position + -_normal * lowerValue;
            var predictedNormal = _normal;
            var predictedVelocity = _rigidbody.linearVelocity;
            var speedFrame = _rigidbody.linearVelocity.magnitude * Time.fixedDeltaTime;
            var lerpJump = 0.015f;
            var mask = _config.castLayer;
            
            if (!Physics.Raycast(predictedPosition, predictedVelocity.normalized, 
                    out var pGround, speedFrame * 1.3f, mask)) { HighSpeedFix(); return; }

            for (var lerp = lerpJump; lerp < 45 / 90; lerp += lerpJump)
            {
                if (!Physics.Raycast(predictedPosition, Vector3.Lerp(predictedVelocity.normalized, _normal, lerp), out pGround, speedFrame * 1.3f, mask))
                {
                    lerp += lerpJump;
                    Physics.Raycast(predictedPosition + Vector3.Lerp(predictedVelocity.normalized, _normal, lerp) * (speedFrame * 1.3f), -predictedNormal, 
                        out pGround, _config.castDistance + 0.2f, mask);

                    predictedPosition = predictedPosition + Vector3.Lerp(predictedVelocity.normalized, _normal, lerp) * speedFrame + pGround.normal * lowerValue;
                    predictedVelocity = Quaternion.FromToRotation(_normal, pGround.normal) * predictedVelocity;
                    _normal = pGround.normal;
                    _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, predictedPosition, Time.fixedDeltaTime);
                    _rigidbody.linearVelocity = predictedVelocity;
                    break;
                }
            }
        }
        
        private void HighSpeedFix()
        {
            var predictedPosition = _rigidbody.position;
            var predictedNormal = actor.stats.groundNormal;
            var predictedVelocity = _rigidbody.linearVelocity;
            var steps = 16;
            var mask = _config.castLayer;
            int i;
            for (i = 0; i < steps; i++)
            {
                predictedPosition += predictedVelocity * Time.fixedDeltaTime / steps;
                if (Physics.Raycast(predictedPosition, -predictedNormal, out var pGround, _config.castDistance + 0.2f, mask))
                {
                    if (Vector3.Angle (predictedNormal, pGround.normal) < 45)
                    {
                        predictedPosition = pGround.point + pGround.normal * 0.5f;
                        predictedVelocity = Quaternion.FromToRotation(actor.stats.groundNormal, pGround.normal) * predictedVelocity;
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
                actor.stats.groundNormal = predictedNormal;
                _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, predictedPosition, Time.fixedDeltaTime);
            }
        }

        #endregion

        public void Project(Vector3 normal = default)
        {
            if (normal == default)
            {
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, _normal);
                return;
            }
            
            _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
        }

        private void BaseAirPhysics()
        {
            float handling = _turnRate;
            handling *= 0.2f;
            _movementVector = Vector3.Lerp(_planarVelocity, _inputDir.normalized * _planarVelocity.magnitude, 
                handling * Time.fixedDeltaTime);
        }

        public void Snap(Vector3 point, Vector3 normal, bool instant = false)
        {
            if (!_canAttach) return;

            if (point != Vector3.zero && normal != Vector3.zero)
            {
                Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, normal) * _rigidbody.rotation;
                Vector3 goal = point + normal;
                _rigidbody.position = Vector3.Lerp(_rigidbody.position, goal, 
                    Time.fixedDeltaTime * (Mathf.Abs(Quaternion.Dot(_rigidbody.rotation, slopeRotation) + 1f) / 2f * _rigidbody.linearVelocity.magnitude + 15f));
            }
        }

        public void Deceleration(float min, float max)
        {
            if (actor.stateMachine.CurrentState is FStateAir)
            {
                return;
            }
            
            if (SonicTools.IsBoost()) return;
            if (actor.flags.HasFlag(FlagType.OutOfControl)) return;
            
            float f = Mathf.Lerp(max, min, 
                _movementVector.magnitude / _config.topSpeed);
            if (_movementVector.magnitude > 0.2f)
                _movementVector = Vector3.MoveTowards(_movementVector, Vector3.zero, Time.fixedDeltaTime * f);
            else
            {
                _movementVector = Vector3.zero;
                actor.stateMachine.SetState<FStateIdle>();
            }
        }
        
        public void SetDetachTime(float t)
        {
            _detachTimer = 0;
            _canAttach = false;
            
            _detachTimer = t;
        }
        
        public void ModifyTurnRate(float modifier) => _turnRate *= modifier;

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

        public bool CheckForPredictedGround(Vector3 vel, Vector3 normal, float deltaTime, float distance, int steps)
        {
            bool willBeGrounded = false;
            Vector3 initVel = vel;
            Vector3 predictedNormal = normal;
            Vector3 predictedPos = _rigidbody.position;
            for (int i = 0; i < steps; i++)
            {
                predictedPos += vel * deltaTime / steps;
                if (Physics.Raycast(predictedPos, -predictedNormal, out RaycastHit hit, 1f + distance, _config.castLayer))
                {
                    float Dot = Vector3.Dot(_rigidbody.linearVelocity, hit.normal);
                    float MaxAngle = Dot < 0 ? maxAngleDifference : 30f;
                    if (Vector3.Angle(predictedNormal, hit.normal) < MaxAngle)
                    {
                        predictedPos = hit.point + hit.normal;
                        predictedNormal = hit.normal;
                        initVel = Quaternion.FromToRotation(_normal, predictedNormal) * initVel;
                        willBeGrounded = true;
                    }
                }
            }

            return willBeGrounded;
        }

        public bool GetAttachState() => _canAttach;
        
        public Vector3 GetInputDir()
        {
            return _inputDir;
        }
        
        public void SetPath(PathData data)
        {
            _pathData = data;
        }

        public void SetAngle() => _angle = Vector3.Angle(_normal, Vector3.up);

        public bool IsPathValid()
        {
            return _pathData != null;
        }
    }

    public class GroundData
    {
        public Vector3 point;
        public Vector3 normal;
        public Transform transform;
        public bool isValid;
    }
}