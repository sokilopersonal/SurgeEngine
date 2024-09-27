using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.Parameters
{
    public class FStateGround : FStateMove
    {
        [SerializeField] private Vector3 _groundCheckOffset;

        private Transform _cameraTransform;
        private const float INPUT_DEADZONE = 0.3f;
        private const float SLOPE_PREDICTION_ACTIVATE_SPEED = 10f;
        
        private float _detachTimer;
        private bool _canAttach;
        private string _surfaceTag;

        public override void OnEnter()
        {
            base.OnEnter();

            _cameraTransform = actor.camera.GetCameraTransform();
            stats.groundNormal = Vector3.up;
            
            SetDetachTime(0f);
            
            //UpdateNormal();
            ConvertAirToGroundVelocity();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            animation.TransitionToState(AnimatorParams.RunCycle, 0f);
            BoostHandle(dt);

            if (actor.input.JumpPressed)
            {
                SetDetachTime(0.2f);

                actor.stateMachine.SetState<FStateJump>();
            }
            
            CalculateDetachState();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _rigidbody.linearVelocity = Vector3.zero;
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Vector3 prevNormal = stats.groundNormal; 
            if (Common.CheckForGround(out var hit))
            {
                var point = hit.point;
                var normal = hit.normal;
                stats.groundNormal = normal;
                
                Vector3 stored = _rigidbody.linearVelocity;
                _rigidbody.linearVelocity = Quaternion.FromToRotation(_rigidbody.transform.up, prevNormal) * stored;
                stats.transformNormal = Vector3.Slerp(stats.transformNormal, normal, dt * 14f);

                if (_rigidbody.linearVelocity.magnitude > SLOPE_PREDICTION_ACTIVATE_SPEED)
                {
                    SlopePrediction(dt);
                }

                Movement(dt);
                actor.model.RotateBody(normal);
                Snap(point, normal);
                
                _surfaceTag = hit.transform.gameObject.GetGroundTag();
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
            }
            else
            {
                stateMachine.SetState<FStateAir>();
            }
            
            var path = actor.bezierPath;
            if (path != null)
            {
                SplineUtility.GetNearestPoint(path.Spline, SurgeMath.Vector3ToFloat3(path.transform.InverseTransformPoint(_rigidbody.position)), out var near, out var t);
                path.Evaluate(t, out var point, out var tangent, out var up);
                var planeNormal = Vector3.Cross(tangent, up);
                
                if (stats.currentSpeed < stats.moveParameters.topSpeed)
                {
                    _rigidbody.AddForce(_rigidbody.transform.forward * (dt * 25), ForceMode.Impulse);
                }
                
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, planeNormal);
                stats.inputDir = Vector3.ProjectOnPlane(stats.inputDir, planeNormal);

                Vector3 nearPoint = path.transform.TransformPoint(near);
                _rigidbody.position = Vector3.Lerp(_rigidbody.position, nearPoint, 8f * dt);
                _rigidbody.rotation = Quaternion.LookRotation(tangent, up);
            }
        }

        private void Movement(float dt)
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            var normal = stats.groundNormal;
            SurgeMath.SplitPlanarVector(velocity, normal, out Vector3 planar, out _);

            stats.movementVector = planar;
            stats.planarVelocity = planar;
            
            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, normal) *
                                       (_cameraTransform.rotation * actor.input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, normal);
            stats.inputDir = transformedInput.normalized * actor.input.moveVector.magnitude;
            if (input.moveVector == Vector3.zero && stateMachine.GetSubState<FBoost>().Active) stats.inputDir = _rigidbody.transform.forward;

            if (stats.inputDir.magnitude > INPUT_DEADZONE)
            {
                if (!stats.skidding)
                {
                    CalculateVelocity(dt);
                }
                else
                {
                    if (stateMachine.GetSubState<FBoost>().Active) return;
                    
                    Deacceleration(stats.moveParameters.skidMaxRate, stats.moveParameters.skidMinRate);
                }
            }
            else
            {
                if (stateMachine.GetSubState<FBoost>().Active) return;

                Deacceleration(stats.moveParameters.maxDeacceleration, stats.moveParameters.minDeacceleration);
            }
            
            SlopePhysics(dt);
            
            Vector3 movementVelocity = stats.movementVector;
            if (!stateMachine.GetSubState<FBoost>().Active)
            {
                movementVelocity = Vector3.Lerp(movementVelocity,
                    Vector3.ClampMagnitude(movementVelocity, stats.moveParameters.maxSpeed),
                    SurgeMath.Smooth(1 - 0.95f));
            } 
            _rigidbody.linearVelocity = movementVelocity;
        }

        private void Deacceleration(float min, float max)
        {
            if (actor.flags.HasFlag(FlagType.OutOfControl)) return;
            
            float f = Mathf.Lerp(max, min, 
                stats.movementVector.magnitude / stats.moveParameters.topSpeed);
            if (stats.movementVector.magnitude > 1f)
                stats.movementVector = Vector3.MoveTowards(stats.movementVector, Vector3.zero, Time.fixedDeltaTime * f);
            else
            {
                stats.movementVector = Vector3.zero;
                    
                stateMachine.SetState<FStateIdle>();
            }
        }

        private void BoostHandle(float dt)
        {
            FBoost boost = stateMachine.GetSubState<FBoost>();
            if (boost.Active && stats.currentSpeed < boost.startForce)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * boost.startForce;
                boost.restoringTopSpeed = true;
                animation.TransitionToState(AnimatorParams.RunCycle, 0f);
            }
    
            if (boost.Active)
            {
                float maxSpeed = stats.moveParameters.maxSpeed * boost.maxSpeedMultiplier;
                if (stats.currentSpeed < maxSpeed) _rigidbody.AddForce(_rigidbody.transform.forward * (boost.boostForce * dt), ForceMode.VelocityChange);
                    
            }
            else if (boost.restoringTopSpeed)
            {
                float normalMaxSpeed = stats.moveParameters.topSpeed;
                if (stats.currentSpeed > normalMaxSpeed)
                {
                    _rigidbody.linearVelocity = Vector3.MoveTowards(
                        _rigidbody.linearVelocity, 
                        _rigidbody.transform.forward * normalMaxSpeed, 
                        dt * boost.restoreSpeed
                    );
                }
                else if (stats.currentSpeed * 0.99f < normalMaxSpeed)
                {
                    boost.restoringTopSpeed = false;
                }
            }
        }

        private void CalculateVelocity(float dt)
        {
            stats.turnRate = Mathf.Lerp(stats.turnRate, stats.moveParameters.turnSpeed
                                                        * (stateMachine.GetSubState<FBoost>().Active ? stateMachine.GetSubState<FBoost>().turnSpeedReduction : 1), dt * stats.moveParameters.turnSmoothing);
            var accelRateMod = stats.moveParameters.accelCurve.Evaluate(stats.planarVelocity.magnitude / stats.moveParameters.topSpeed);
            if (stats.planarVelocity.magnitude < stats.moveParameters.topSpeed)
                stats.planarVelocity += stats.inputDir * (stats.moveParameters.accelRate * accelRateMod * dt);
            
            Vector3 newVelocity = Quaternion.FromToRotation(stats.planarVelocity.normalized, stats.inputDir.normalized) * stats.planarVelocity;
            float handling = stats.turnRate;
            handling *= stats.moveParameters.turnCurve.Evaluate(stats.planarVelocity.magnitude / stats.moveParameters.topSpeed);
            stats.movementVector = Vector3.Slerp(stats.planarVelocity, newVelocity, Time.fixedDeltaTime * handling);
        }

        protected virtual void SlopePhysics(float dt)
        {
            stats.groundAngle = Vector3.Angle(stats.groundNormal, Vector3.up);
            if (stats.currentSpeed < 10 && stats.groundAngle >= 70)
            {
                SetDetachTime(0.5f);
                _rigidbody.AddForce(stats.groundNormal * 8f, ForceMode.Impulse);
            }
            
            float dot = Vector3.Dot(transform.up, Vector3.up);
            if (stats.groundAngle > 5 && stats.movementVector.magnitude > 10f)
            {
                bool uphill = Vector3.Dot(_rigidbody.linearVelocity.normalized, Vector3.down) < 0;
                //float groundSpeedMod = slopeForceOverSpeed.Evaluate(rb.velocity.sqrMagnitude / topSpeed / topSpeed);
                Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, stats.groundNormal) * (1 * (uphill ? 1f : 6.5f));
                _rigidbody.linearVelocity += slopeForce * Time.fixedDeltaTime;
            }
            
            float rDot = Vector3.Dot(Vector3.up, actor.transform.right);
            if (Mathf.Abs(rDot) > 0.1f && stats.groundAngle == 90)
            {
                _rigidbody.linearVelocity += Vector3.down * (9f * dt);
            }
        }

        private void Snap(Vector3 point, Vector3 normal) // TODO: Fix snapping
        {
            if (!_canAttach) return;
            
            _rigidbody.position = point + normal;
            //_rigidbody.position = Vector3.Slerp(_rigidbody.position, point + normal, 20 * Time.fixedDeltaTime);
        }

        private void SlopePrediction(float dt)
        {
            var lowerValue = 0.43f;
            var stats = actor.stats;
            var predictedPosition = _rigidbody.position + -stats.groundNormal * lowerValue;
            var predictedNormal = stats.groundNormal;
            var predictedVelocity = _rigidbody.linearVelocity;
            var speedFrame = _rigidbody.linearVelocity.magnitude * dt;
            var lerpJump = 0.015f;
            var mask = stats.moveParameters.castParameters.collisionMask;
            
            if (!Physics.Raycast(predictedPosition, predictedVelocity.normalized, 
                    out var pGround, speedFrame * 1.3f, mask)) { HighSpeedFix(dt); return; }

            for (var lerp = lerpJump; lerp < 45 / 90; lerp += lerpJump)
            {
                if (!Physics.Raycast(predictedPosition, Vector3.Lerp(predictedVelocity.normalized, stats.groundNormal, lerp), out pGround, speedFrame * 1.3f, mask))
                {
                    lerp += lerpJump;
                    Physics.Raycast(predictedPosition + Vector3.Lerp(predictedVelocity.normalized, stats.groundNormal, lerp) * (speedFrame * 1.3f), -predictedNormal, 
                        out pGround, stats.moveParameters.castParameters.castDistance + 0.2f, mask);

                    predictedPosition = predictedPosition + Vector3.Lerp(predictedVelocity.normalized, stats.groundNormal, lerp) * speedFrame + pGround.normal * lowerValue;
                    predictedVelocity = Quaternion.FromToRotation(stats.groundNormal, pGround.normal) * predictedVelocity;
                    stats.groundNormal = pGround.normal;
                    _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, predictedPosition, dt);
                    _rigidbody.linearVelocity = predictedVelocity;
                    break;
                }
            }
        }
         
        private void HighSpeedFix(float dt)
        {
            var predictedPosition = _rigidbody.position;
            var predictedNormal = actor.stats.groundNormal;
            var predictedVelocity = _rigidbody.linearVelocity;
            var steps = 8;
            var mask = stats.moveParameters.castParameters.collisionMask;
            int i;
            for (i = 0; i < steps; i++)
            {
                predictedPosition += predictedVelocity * dt / steps;
                if (Physics.Raycast(predictedPosition, -predictedNormal, out var pGround, stats.moveParameters.castParameters.castDistance + 0.2f, mask))
                {
                    if (Vector3.Angle (predictedNormal, pGround.normal) < 45)
                    {
                        predictedPosition = pGround.point + pGround.normal * 0.5f;
                        predictedVelocity = Quaternion.FromToRotation(actor.stats.groundNormal, pGround.normal) * predictedVelocity;
                        predictedNormal = pGround.normal;
                    } else
                    {
                        i = -1;
                        break;
                    }
                } else
                {
                    i = -1;
                    break;
                }
            }
            if (i >= steps)
            {
                actor.stats.groundNormal = predictedNormal;
                _rigidbody.position = Vector3.MoveTowards(_rigidbody.position, predictedPosition, dt);
            }
        }

        private void UpdateNormal()
        {
            if (Physics.Raycast(actor.transform.position, -actor.transform.up, out var hit,
                    stats.moveParameters.castParameters.castDistance, stats.moveParameters.castParameters.collisionMask))
            {
                var point = hit.point;
                var normal = hit.normal;
                stats.groundNormal = normal;

                _rigidbody.position = point + normal;
            }
        }
        
        private void ConvertAirToGroundVelocity()
        {
            if (Physics.Raycast(actor.transform.position, _rigidbody.linearVelocity.normalized, out RaycastHit velocityFix, _rigidbody.linearVelocity.magnitude, stats.moveParameters.castParameters.collisionMask))
            {
                float nextGroundAngle = Vector3.Angle(velocityFix.normal, Vector3.up);
                if (nextGroundAngle <= 20)
                {
                    Vector3 fixedVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, actor.transform.up);
                    fixedVelocity = Quaternion.FromToRotation(actor.transform.up, velocityFix.normal) * fixedVelocity;
                    _rigidbody.linearVelocity = fixedVelocity;
                }
            }
        }

        public void SetDetachTime(float t)
        {
            _detachTimer = 0;
            _canAttach = false;
            
            _detachTimer = t;
        }

        public void CalculateDetachState()
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

        public void SetAttachState(bool value) => _canAttach = value; 
        public bool GetAttachState() => _canAttach;
        public string GetSurfaceTag() => _surfaceTag;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + _groundCheckOffset, 0.1f);
            
            Gizmos.DrawRay(transform.position + _groundCheckOffset, -transform.up * 1.5f);
        }
    }
}