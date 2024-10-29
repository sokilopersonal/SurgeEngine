using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;
using static SurgeEngine.Code.GameDocuments.SonicGameDocumentParams;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorKinematics : ActorComponent
    {
        public Rigidbody Rigidbody => _rigidbody;

        public float TurnRate
        {
            get => _turnRate;
            set => _turnRate = value;
        }
        public float HorizontalSpeed => _horizontalSpeed;

        public float Angle => _angle;

        public Vector3 Normal
        {
            get => _normal;
            set => _normal = value;
        }

        private Vector3 _inputDir;
        private Rigidbody _rigidbody;
        private Transform _cameraTransform;

        private Vector3 _movementVector;
        private Vector3 _planarVelocity;
        private Vector3 _normal;

        private float _horizontalSpeed;
        private float _speed;
        private float _turnRate;
        private float _angle;
        private float _detachTimer;
        private bool _canAttach;
        private bool _skidding;

        private Document _document;
        private ParameterGroup _physGroup;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _rigidbody = GetComponent<Rigidbody>();
            _cameraTransform = actor.camera.GetCameraTransform();

            _document = SonicGameDocument.GetDocument("Sonic");
            _physGroup = _document.GetGroup(SonicGameDocument.PhysicsGroup);
        }

        private void Update()
        {
            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, _normal) *
                                       (_cameraTransform.rotation * actor.input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, _normal);
            _inputDir = transformedInput.normalized * actor.input.moveVector.magnitude;
            if (actor.input.moveVector == Vector3.zero && actor.stateMachine.GetSubState<FBoost>().Active) _inputDir = _rigidbody.transform.forward;
            
            _skidding = actor.stats.moveDot < _physGroup.GetParameter<float>(BasePhysics_SkiddingThreshold);
            _horizontalSpeed = _rigidbody.GetHorizontalMagnitude();
            _speed = _rigidbody.linearVelocity.magnitude;
            
            CalculateDetachState();
        }

        public void BasePhysics(Vector3 point, Vector3 normal)
        {
            Vector3 vel = _rigidbody.linearVelocity;
            Vector3 dir = _inputDir;
            SurgeMath.SplitPlanarVector(vel, normal, out Vector3 planar, out Vector3 vertical); 
            
            _movementVector = planar;
            _planarVelocity = planar;
            
            var state = actor.stateMachine.CurrentState;
            var param = _physGroup;

            if (_inputDir.magnitude > 0.2f)
            {
                if (state is FStateSliding)
                {
                    bool canMove = actor.stats.moveDot > -0.1f && actor.stats.moveDot < 0.975f;
                    if (canMove)
                    {
                        _turnRate *= 0.02f;
                    }
                    else
                    {
                        dir = Vector3.zero;
                    }
                }
                
                if (!_skidding)
                {
                    _turnRate = Mathf.Lerp(_turnRate, param.GetParameter<float>(BasePhysics_TurnSpeed), 
                        param.GetParameter<float>(BasePhysics_TurnSmoothing) * Time.fixedDeltaTime);
                    
                    var accelRateMod = param.GetParameter<AnimationCurve>(BasePhysics_AccelerationCurve)
                        .Evaluate(_planarVelocity.magnitude / param.GetParameter<float>(BasePhysics_TopSpeed));
                    if (_planarVelocity.magnitude < param.GetParameter<float>(BasePhysics_TopSpeed))
                        _planarVelocity += dir * (param.GetParameter<float>(BasePhysics_AccelerationRate) * accelRateMod * Time.fixedDeltaTime);

                    switch (state)
                    {
                        case FStateGround or FStateSliding:
                            BaseGroundPhysics(param);
                            break;
                        case FStateAir or FStateJump:
                            BaseAirPhysics();
                            break;
                    }
                }
                else
                {
                    Deceleration(param.GetParameter<float>(BasePhysics_MinSkiddingRate), param.GetParameter<float>(BasePhysics_MaxSkiddingRate));
                }
            }
            else
            {
                Deceleration(param.GetParameter<float>(BasePhysics_MinDeaccelerationRate), param.GetParameter<float>(BasePhysics_MaxDeaccelerationRate));
            }
            
            if (actor.stats.lastContactObject is not TrickJumper or Spring or WideSpring or DashRing or RainbowRing) 
                vertical = Vector3.ClampMagnitude(vertical, param.GetParameter<float>(BasePhysics_MaxVerticalSpeed));
            _rigidbody.linearVelocity = _movementVector + vertical;
            
            Snap(point, normal);
        }

        private void BaseGroundPhysics(ParameterGroup param)
        {
            Vector3 newVelocity = Quaternion.FromToRotation(_planarVelocity.normalized, _inputDir.normalized) * _planarVelocity;
            float handling = _turnRate;
            handling *= param.GetParameter<AnimationCurve>(BasePhysics_TurnCurve).Evaluate(_planarVelocity.magnitude / param.GetParameter<float>(BasePhysics_TopSpeed));
            _movementVector = Vector3.Slerp(_planarVelocity, newVelocity, handling * Time.fixedDeltaTime);
            
            SlopePhysics();
            
            Project();
        }

        public void SlopePhysics()
        {
            SlopePrediction();
            
            var doc = GameDocument<SonicGameDocument>.GetDocument("Sonic");
            var param = doc.GetGroup("Slope");
            _angle = Vector3.Angle(_normal, Vector3.up);
            if (_speed < param.GetParameter<float>(Slope_MinSpeed) && _angle >= param.GetParameter<float>(Slope_DeslopeAngle))
            {
                _rigidbody.AddForce(_normal * param.GetParameter<float>(Slope_DeslopeForce), ForceMode.Impulse);
                actor.stateMachine.SetState<FStateAir>(param.GetParameter<float>(Slope_InactiveDuration));
            }
            
            if (_angle > param.GetParameter<float>(Slope_MinAngle) && _speed > param.GetParameter<float>(Slope_MinForceSpeed))
            {
                bool uphill = Vector3.Dot(_rigidbody.linearVelocity.normalized, Vector3.down) < 0;
                Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, _normal) * (1 * (uphill ? param.GetParameter<float>(Slope_UphillForce) : param.GetParameter<float>(Slope_DownhillForce)));
                _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
            }
            
            float rDot = Vector3.Dot(Vector3.up, actor.transform.right);
            if (Mathf.Abs(rDot) > 0.1f && Mathf.Approximately(_angle, 90))
            {
                _rigidbody.linearVelocity += Vector3.down * (param.GetParameter<float>(Slope_WallGravity) * Time.fixedDeltaTime);
            }
        }

        public void SlopePrediction()
        {
            var lowerValue = 0.45f;
            var param = _document.GetGroup("Cast");
            var predictedPosition = _rigidbody.position + -_normal * lowerValue;
            var predictedNormal = _normal;
            var predictedVelocity = _rigidbody.linearVelocity;
            var speedFrame = _rigidbody.linearVelocity.magnitude * Time.fixedDeltaTime;
            var lerpJump = 0.015f;
            var mask = param.GetParameter<LayerMask>(Cast_Mask);
            
            if (!Physics.Raycast(predictedPosition, predictedVelocity.normalized, 
                    out var pGround, speedFrame * 1.3f, mask)) { HighSpeedFix(); return; }

            for (var lerp = lerpJump; lerp < 45 / 90; lerp += lerpJump)
            {
                if (!Physics.Raycast(predictedPosition, Vector3.Lerp(predictedVelocity.normalized, _normal, lerp), out pGround, speedFrame * 1.3f, mask))
                {
                    lerp += lerpJump;
                    Physics.Raycast(predictedPosition + Vector3.Lerp(predictedVelocity.normalized, _normal, lerp) * (speedFrame * 1.3f), -predictedNormal, 
                        out pGround, param.GetParameter<float>(Cast_Distance) + 0.2f, mask);

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
            var param = _document.GetGroup("Cast");
            var steps = 16;
            var mask = param.GetParameter<LayerMask>(Cast_Mask);
            int i;
            for (i = 0; i < steps; i++)
            {
                predictedPosition += predictedVelocity * Time.fixedDeltaTime / steps;
                if (Physics.Raycast(predictedPosition, -predictedNormal, out var pGround, param.GetParameter<float>(Cast_Distance) + 0.2f, mask))
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

        public void Project()
        {
            _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, _normal);
        }

        private void BaseAirPhysics()
        {
            float handling = _turnRate;
            handling *= 0.075f;
            _movementVector = Vector3.Lerp(_planarVelocity, _inputDir.normalized * _planarVelocity.magnitude, 
                handling * Time.fixedDeltaTime);
        }

        public void Snap(Vector3 point, Vector3 normal)
        {
            if (!_canAttach) return;
            
            if (point != Vector3.zero && normal != Vector3.zero) _rigidbody.position = Vector3.Slerp(_rigidbody.position, point + normal, Time.fixedDeltaTime * 10f);
        }

        private void Deceleration(float min, float max)
        {
            if (actor.stateMachine.CurrentState is FStateAir or FStateSliding)
            {
                return;
            }
            
            if (actor.stateMachine.GetSubState<FBoost>().Active) return;
            if (actor.flags.HasFlag(FlagType.OutOfControl)) return;
            
            float f = Mathf.Lerp(max, min, 
                _movementVector.magnitude / _physGroup.GetParameter<float>(BasePhysics_TopSpeed));
            if (_movementVector.magnitude > 0.5f)
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
    }
}