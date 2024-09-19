using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateGround : FStateMove
    {
        [SerializeField] private Vector3 _groundCheckOffset;

        private Transform _cameraTransform;
        private const float INPUT_DEADZONE = 0.3f;
        private const float SLOPE_PREDICTION_ACTIVATE_SPEED = 10f;

        public override void OnEnter()
        {
            base.OnEnter();
            
            animation.TransitionToState(AnimatorParams.RunCycle, 0.1f);

            stats.groundNormal = Vector3.up;
            _cameraTransform = actor.camera.GetCameraTransform();
            
            UpdateNormal();
            ConvertAirToGroundVelocity();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            FBoost boost = stateMachine.GetSubState<FBoost>();
            if (input.BoostPressed && stats.currentSpeed < boost.startForce)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * boost.startForce;
                boost.restoringTopSpeed = true;
                animation.TransitionToState(AnimatorParams.RunCycle, 0f);
            }
    
            if (stats.boost.Active)
            {
                _rigidbody.AddForce(Vector3.ProjectOnPlane(_rigidbody.transform.forward, stats.groundNormal) * (boost.boostForce * dt), ForceMode.Impulse);
                float maxSpeed = stats.moveParameters.maxSpeed * boost.maxSpeedMultiplier;
                _rigidbody.linearVelocity = Vector3.ClampMagnitude(_rigidbody.linearVelocity, maxSpeed);
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

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _rigidbody.linearVelocity = Vector3.zero;
            }
        }


        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Physics.SphereCast(actor.transform.position, 0.1f, -actor.transform.up, out var hit,
                    stats.moveParameters.castParameters.castDistance, stats.moveParameters.castParameters.collisionMask))
            {
                var point = hit.point;
                var normal = hit.normal;
                stats.groundNormal = normal;
                
                stats.transformNormal = Vector3.Slerp(stats.transformNormal, normal, dt * 14f);

                if (_rigidbody.linearVelocity.magnitude > SLOPE_PREDICTION_ACTIVATE_SPEED)
                {
                    SlopePrediction(dt);
                }

                Movement(dt);
                SlopePhysics();
                Rotate(dt);
                Snap(point, normal);
                
                //_rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
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
            if (input.moveVector == Vector3.zero && stats.boost.Active) stats.inputDir = _rigidbody.transform.forward;

            if (stats.inputDir.magnitude > INPUT_DEADZONE)
            {
                CalculateVelocity(dt);
            }
            else
            {
                if (stateMachine.GetSubState<FBoost>().Active) return;
                
                float f = Mathf.Lerp(stats.moveParameters.maxDeacceleration, stats.moveParameters.minDeacceleration, 
                    stats.movementVector.magnitude / stats.moveParameters.topSpeed);
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

        private void CalculateVelocity(float dt)
        {
            stats.turnRate = Mathf.Lerp(stats.turnRate, stats.moveParameters.turnSpeed
                                                        * (stats.boost.Active ? stats.boost.turnSpeedReduction : 1), dt * stats.moveParameters.turnSmoothing);
            var accelRateMod = stats.moveParameters.accelCurve.Evaluate(stats.planarVelocity.magnitude / stats.moveParameters.topSpeed);
            if (stats.planarVelocity.magnitude < stats.moveParameters.topSpeed)
                stats.planarVelocity += stats.inputDir * (stats.moveParameters.accelRate * accelRateMod * dt);
                
            Vector3 newVelocity = Quaternion.FromToRotation(stats.planarVelocity.normalized, stats.inputDir.normalized) * stats.planarVelocity;
            float handling = stats.turnRate;
            handling *= stats.moveParameters.turnCurve.Evaluate(stats.planarVelocity.magnitude / stats.moveParameters.topSpeed);
            stats.movementVector = Vector3.Slerp(stats.planarVelocity, newVelocity, Time.fixedDeltaTime * handling);
        }

        protected virtual void SlopePhysics()
        {
            stats.groundAngle = Vector3.Angle(stats.groundNormal, Vector3.up);
            // if (_rigidbody.linearVelocity.magnitude < 5 && stats.groundAngle >= 75)
            // {
            //     _rigidbody.AddForce(groundNormal * 0.15f, ForceMode.Impulse);
            //     groundNormal = Vector3.up;
            //     grounded = false;
            // }
            
            float dot = Vector3.Dot(transform.up, Vector3.up);
            if (stats.groundAngle > 5 && stats.movementVector.magnitude > 10f)
            {
                bool uphill = Vector3.Dot(_rigidbody.linearVelocity.normalized, Vector3.down) < 0;
                //float groundSpeedMod = slopeForceOverSpeed.Evaluate(rb.velocity.sqrMagnitude / topSpeed / topSpeed);
                Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, stats.groundNormal) * (1 * (uphill ? 1f : 6.5f));
                _rigidbody.linearVelocity += slopeForce * Time.fixedDeltaTime;
            }
        }

        private void Snap(Vector3 point, Vector3 normal)
        {
            _rigidbody.position = Vector3.Slerp(_rigidbody.position, point + normal, 12 * Time.fixedDeltaTime);
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
                    Physics.Raycast(predictedPosition + Vector3.Lerp(predictedVelocity.normalized, stats.groundNormal, lerp) * (speedFrame * 1.3f), -predictedNormal, out pGround, stats.moveParameters.castParameters.castDistance + 0.2f, mask);

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

        private void Rotate(float dt)
        {
            stats.transformNormal = stats.groundNormal;

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, stats.groundNormal);

            if (vel.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, stats.transformNormal);
                actor.transform.rotation = Quaternion.Slerp(actor.transform.rotation, rot, dt * 10);
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + _groundCheckOffset, 0.1f);
            
            Gizmos.DrawRay(transform.position + _groundCheckOffset, -transform.up * 1.5f);
        }
    }
}