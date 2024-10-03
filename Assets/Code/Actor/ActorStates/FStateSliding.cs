using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateSliding : FStateMove
    {
        [SerializeField] private float collisionHeight = 0.3f;
        [SerializeField] private float collisionCenterY = -0.5f;
        [SerializeField] private float slideDeacceleration = 10f;
        public float slideDeactivationSpeed = 7f;

        private Transform _cameraTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            
            stateMachine.GetSubState<FBoost>().Active = false;

            _cameraTransform = actor.camera.GetCameraTransform();
            
            actor.model.collision.height = collisionHeight;
            actor.model.collision.center = new Vector3(0, collisionCenterY, 0);
        }

        public override void OnExit()
        {
            base.OnExit();

            actor.model.collision.height = actor.model.collisionStartHeight;
            actor.model.collision.center = Vector3.zero;
            
            animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (stats.currentSpeed < slideDeactivationSpeed || !input.BHeld)
            {
                if (stats.currentSpeed > slideDeactivationSpeed)
                {
                    stateMachine.SetState<FStateGround>();
                }
                else
                    stateMachine.SetState<FStateIdle>();
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
                _rigidbody.position = point + normal;
                
                Vector3 stored = _rigidbody.linearVelocity;
                _rigidbody.linearVelocity = Quaternion.FromToRotation(_rigidbody.transform.up, prevNormal) * stored;
                stats.transformNormal = Vector3.Slerp(stats.transformNormal, normal, dt * 14f);
                
                actor.model.RotateBody(normal);
                
                Movement(dt); 
                actor.model.RotateBody(normal);
                
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
                
                stats.groundAngle = Vector3.Angle(stats.groundNormal, Vector3.up);
                if (stats.currentSpeed < 10 && stats.groundAngle >= 70)
                {
                    stateMachine.GetState<FStateGround>().SetDetachTime(0.5f);
                    _rigidbody.AddForce(stats.groundNormal * 8f, ForceMode.Impulse);
                }
            
                if (stats.groundAngle > 5 && stats.movementVector.magnitude > 10f)
                {
                    bool uphill = Vector3.Dot(_rigidbody.linearVelocity.normalized, Vector3.down) < 0;
                    Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, stats.groundNormal) * (1 * (uphill ? 4f : 50f));
                    _rigidbody.linearVelocity += slopeForce * Time.fixedDeltaTime;
                }
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
            
            CalculateVelocity(dt);
            Deacceleration();
            
            Vector3 movementVelocity = stats.movementVector;
            if (!stateMachine.GetSubState<FBoost>().Active)
            {
                movementVelocity = Vector3.Lerp(movementVelocity,
                    Vector3.ClampMagnitude(movementVelocity, stats.moveParameters.maxSpeed),
                    SurgeMath.Smooth(1 - 0.95f));
            } 
            _rigidbody.linearVelocity = movementVelocity;
        }

        private void CalculateVelocity(float dt)
        {
            bool canMove = stats.moveDot > -0.1f && stats.moveDot < 0.975f;
            if (canMove)
            {
                stats.turnRate = Mathf.Lerp(stats.turnRate, stats.moveParameters.turnSpeed, dt * stats.moveParameters.turnSmoothing);
                var accelRateMod = stats.moveParameters.accelCurve.Evaluate(stats.planarVelocity.magnitude / stats.moveParameters.topSpeed);
                if (stats.planarVelocity.magnitude < stats.moveParameters.topSpeed)
                {
                    stats.planarVelocity += stats.inputDir * (stats.moveParameters.accelRate * accelRateMod * dt);
                }
                float handling = stats.turnRate * 0.02f;
                stats.movementVector = Vector3.Lerp(stats.planarVelocity, stats.inputDir * stats.planarVelocity.magnitude, 
                    dt * handling);
            }
        }

        private void Deacceleration()
        {
            if (actor.flags.HasFlag(FlagType.OutOfControl)) return;

            if (stats.movementVector.magnitude > 1f)
                stats.movementVector = Vector3.MoveTowards(stats.movementVector, Vector3.zero, Time.fixedDeltaTime * slideDeacceleration);
            else
            {
                stats.movementVector = Vector3.zero;
                stateMachine.SetState<FStateIdle>();
            }
        }

    }
}