using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateGround : FStateMove
    {
        [SerializeField] private MoveParameters moveParameters;
        [SerializeField] private Vector3 _groundCheckOffset;

        private Transform _cameraTransform;

        public override void OnEnter()
        {
            base.OnEnter();

            actor.stats.groundNormal = Vector3.up;
            _cameraTransform = actor.camera.GetCameraTransform();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Physics.Raycast(actor.transform.position, -actor.transform.up, out var hit,
                    1.5f, LayerMask.GetMask("Default")))
            {
                var point = hit.point;
                var normal = hit.normal;
                actor.stats.groundNormal = normal;

                if (_rigidbody.linearVelocity.magnitude > 10f)
                {
                    SlopePrediction(dt);
                }

                var target = point + normal;
                Vector3 lerped = Vector3.Lerp(_rigidbody.position, target, 8 * Time.fixedDeltaTime);
                _rigidbody.position = lerped;
                //_rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, actor.stats.groundNormal);
                
                Movement(dt);
                Rotate(dt);
            }
            else
            {
                actor.stateMachine.SetState<FStateAir>();
            }
        }

        private void Movement(float dt)
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            var stats = actor.stats;
            var normal = stats.groundNormal;
            SurgeMath.SplitPlanarVector(velocity, normal, out Vector3 planar, out _);

            stats.movementVector = planar;
            stats.planarVelocity = planar;

            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, normal) *
                                       (_cameraTransform.rotation * actor.input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, normal);
            stats.inputDir = transformedInput.normalized * actor.input.moveVector.magnitude;

            if (stats.inputDir.magnitude > 0.2f)
            {
                stats.turnRate = Mathf.Lerp(stats.turnRate, moveParameters.turnSpeed, dt * moveParameters.turnSmoothing);
                var accelRateMod = moveParameters.accelCurve.Evaluate(stats.planarVelocity.magnitude / moveParameters.topSpeed);
                if (stats.planarVelocity.magnitude < moveParameters.topSpeed)
                    stats.planarVelocity += stats.inputDir * (moveParameters.accelRate * accelRateMod * dt);
                float handling = stats.turnRate;
                handling *= moveParameters.turnCurve.Evaluate(stats.planarVelocity.magnitude / moveParameters.topSpeed);
                stats.movementVector = Vector3.Lerp(stats.planarVelocity, stats.inputDir.normalized * stats.planarVelocity.magnitude, 
                    dt * handling);
            }
            else
            {
                float f = Mathf.Lerp(12, 4, stats.movementVector.magnitude / moveParameters.topSpeed);
                if (stats.movementVector.magnitude > 1f)
                    stats.movementVector = Vector3.MoveTowards(stats.movementVector, Vector3.zero, Time.fixedDeltaTime * f);
                else
                {
                    stats.movementVector = Vector3.zero;
                    
                    actor.stateMachine.SetState<FStateIdle>();
                }
            }
            
            Vector3 movementVelocity = stats.movementVector;
            _rigidbody.linearVelocity = movementVelocity;
        }
        
        private void SlopePrediction(float dt)
        {
            float lowerValue = 0.43f;
            var stats = actor.stats;
            Vector3 predictedPosition = _rigidbody.position + -stats.groundNormal * lowerValue;
            Vector3 predictedNormal = stats.groundNormal;
            Vector3 predictedVelocity = _rigidbody.linearVelocity;
            float speedFrame = _rigidbody.linearVelocity.magnitude * dt;
            float lerpJump = 0.015f;
            
            if (!Physics.Raycast(predictedPosition, predictedVelocity.normalized, out RaycastHit pGround, speedFrame * 1.3f, LayerMask.GetMask("Default"))) { HighSpeedFix(dt); return; }

            for (float lerp = lerpJump; lerp < 45 / 90; lerp += lerpJump)
            {
                if (!Physics.Raycast(predictedPosition, Vector3.Lerp(predictedVelocity.normalized, stats.groundNormal, lerp), out pGround, speedFrame * 1.3f, LayerMask.GetMask("Default")))
                {
                    lerp += lerpJump;
                    Physics.Raycast(predictedPosition + Vector3.Lerp(predictedVelocity.normalized, stats.groundNormal, lerp) * (speedFrame * 1.3f), -predictedNormal, out pGround, 1.25f + 0.2f, LayerMask.GetMask("Default"));

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
            Vector3 predictedPosition = _rigidbody.position;
            Vector3 predictedNormal = actor.stats.groundNormal;
            Vector3 predictedVelocity = _rigidbody.linearVelocity;
            int steps = 8;
            int i;
            for (i = 0; i < steps; i++)
            {
                predictedPosition += predictedVelocity * dt / steps;
                if (Physics.Raycast(predictedPosition, -predictedNormal, out RaycastHit pGround, 1.25f + 0.2f, LayerMask.GetMask("Default")))
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
        
        public float GetSignedAngle()
        {
            var stats = actor.stats;
            return Vector3.SignedAngle(stats.planarVelocity, stats.inputDir, stats.groundNormal);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + _groundCheckOffset, 0.1f);
            
            Gizmos.DrawRay(transform.position + _groundCheckOffset, -transform.up * 1.5f);
        }
    }
}