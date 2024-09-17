using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateGround : FStateMove
    {
        [SerializeField] public MoveParameters moveParameters;
        [SerializeField] private Vector3 _groundCheckOffset;

        private Transform _cameraTransform;
        private const float INPUT_DEADZONE = 0.2f;
        private const float SLOPE_PREDICTION_ACTIVATE_SPEED = 10f;

        public override void OnEnter()
        {
            base.OnEnter();
            
            animation.TransitionToState(AnimatorParams.RunCycle, 0.1f);

            stats.groundNormal = Vector3.up;
            _cameraTransform = actor.camera.GetCameraTransform();
            
            ConvertAirToGroundVelocity();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (actor.input.BoostPressed)
            {
                float maxSpeed = moveParameters.maxSpeed * (stats.boost.active ? stats.boost.maxSpeedMultiplier : 1f);
                _rigidbody.linearVelocity = _rigidbody.transform.forward * maxSpeed;
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Physics.Raycast(actor.transform.position, -actor.transform.up, out var hit,
                    moveParameters.castParameters.castDistance, moveParameters.castParameters.collisionMask))
            {
                var point = hit.point;
                var normal = hit.normal;
                stats.groundNormal = normal;

                if (_rigidbody.linearVelocity.magnitude > SLOPE_PREDICTION_ACTIVATE_SPEED)
                {
                    SlopePrediction(dt);
                }

                var target = point + normal;
                Vector3 lerped = Vector3.Lerp(_rigidbody.position, target, 13 * Time.fixedDeltaTime);
                _rigidbody.position = lerped;
                
                Movement(dt);
                Rotate(dt);
                
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
            }
            else
            {
                stateMachine.SetState<FStateAir>();
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

            if (stats.inputDir.magnitude > INPUT_DEADZONE)
            {
                stats.turnRate = Mathf.Lerp(stats.turnRate, moveParameters.turnSpeed
                                                            * (stats.boost.active ? stats.boost.turnSpeedReduction : 1), dt * moveParameters.turnSmoothing);
                var accelRateMod = moveParameters.accelCurve.Evaluate(stats.planarVelocity.magnitude / moveParameters.topSpeed);
                if (stats.planarVelocity.magnitude < moveParameters.topSpeed)
                    stats.planarVelocity += stats.inputDir * (moveParameters.accelRate * accelRateMod * dt);
                
                var maxSpeed = moveParameters.maxSpeed * (stats.boost.active ? stats.boost.maxSpeedMultiplier : 1);
                stats.planarVelocity = Vector3.ClampMagnitude(stats.planarVelocity, maxSpeed);
                
                // float handling = stats.turnRate;
                // handling *= moveParameters.turnCurve.Evaluate(stats.planarVelocity.magnitude / moveParameters.topSpeed);
                // stats.movementVector = Vector3.Lerp(stats.planarVelocity, stats.inputDir.normalized * stats.planarVelocity.magnitude, 
                //     dt * handling);
                
                Vector3 newVelocity = Quaternion.FromToRotation(stats.planarVelocity.normalized, stats.inputDir.normalized) * stats.planarVelocity;
                float handling = stats.turnRate;
                handling *= moveParameters.turnCurve.Evaluate(stats.planarVelocity.magnitude / moveParameters.topSpeed);
                stats.movementVector = Vector3.Slerp(stats.planarVelocity, newVelocity, Time.fixedDeltaTime * handling);
            }
            else
            {
                if (stats.boost.active) return;
                
                float f = Mathf.Lerp(12, 4, stats.movementVector.magnitude / moveParameters.topSpeed);
                if (stats.movementVector.magnitude > 1f)
                    stats.movementVector = Vector3.MoveTowards(stats.movementVector, Vector3.zero, Time.fixedDeltaTime * f);
                else
                {
                    stats.movementVector = Vector3.zero;
                    
                    stateMachine.SetState<FStateIdle>();
                }
            }
            
            Vector3 movementVelocity = stats.movementVector;
            _rigidbody.linearVelocity = movementVelocity;
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
            var mask = moveParameters.castParameters.collisionMask;
            
            if (!Physics.Raycast(predictedPosition, predictedVelocity.normalized, 
                    out var pGround, speedFrame * 1.3f, mask)) { HighSpeedFix(dt); return; }

            for (var lerp = lerpJump; lerp < 45 / 90; lerp += lerpJump)
            {
                if (!Physics.Raycast(predictedPosition, Vector3.Lerp(predictedVelocity.normalized, stats.groundNormal, lerp), out pGround, speedFrame * 1.3f, mask))
                {
                    lerp += lerpJump;
                    Physics.Raycast(predictedPosition + Vector3.Lerp(predictedVelocity.normalized, stats.groundNormal, lerp) * (speedFrame * 1.3f), -predictedNormal, out pGround, moveParameters.castParameters.castDistance + 0.2f, mask);

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
            var mask = moveParameters.castParameters.collisionMask;
            int i;
            for (i = 0; i < steps; i++)
            {
                predictedPosition += predictedVelocity * dt / steps;
                if (Physics.Raycast(predictedPosition, -predictedNormal, out var pGround, moveParameters.castParameters.castDistance + 0.2f, mask))
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

        private void Rotate(float dt)
        {
            var stats = actor.stats;
            stats.transformNormal = Vector3.Slerp(stats.transformNormal, stats.groundNormal, dt * 8);

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, stats.groundNormal);

            if (vel.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, stats.transformNormal);
                actor.transform.rotation = Quaternion.Slerp(actor.transform.rotation, rot, dt * 12);
            }
        }
        
        private void ConvertAirToGroundVelocity()
        {
            if (Physics.Raycast(actor.transform.position, _rigidbody.linearVelocity.normalized, out RaycastHit velocityFix, _rigidbody.linearVelocity.magnitude, moveParameters.castParameters.collisionMask))
            {
                //Check if the angle is good
                float nextGroundAngle = Vector3.Angle(velocityFix.normal, Vector3.up);
                if (nextGroundAngle <= 20)
                {
                    Vector3 fixedVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, actor.transform.up);
                    fixedVelocity = Quaternion.FromToRotation(actor.transform.up, velocityFix.normal) * fixedVelocity;
                    _rigidbody.linearVelocity = fixedVelocity;
                }
            }

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + _groundCheckOffset, 0.1f);
            
            Gizmos.DrawRay(transform.position + _groundCheckOffset, -transform.up * 1.5f);
        }
    }
}