using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SonicSubStates.Boost;
using UnityEngine;
using static SurgeEngine.Code.GameDocuments.SonicGameDocument;
using static SurgeEngine.Code.GameDocuments.SonicGameDocumentParams;

namespace SurgeEngine.Code.Parameters
{
    public class FStateAir : FStateMove, IBoostHandler
    {
        private Transform _cameraTransform;

        private float _airTime;
        
        public override void OnEnter()
        {
            base.OnEnter();

            _airTime = 0f;
            
            actor.stats.groundNormal = Vector3.up;
            _cameraTransform = actor.camera.GetCameraTransform();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            stats.homingTarget = null;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            CalculateAirTime(dt);
            stateMachine.GetState<FStateGround>().CalculateDetachState();
            
            if (GetAirTime() > 0.1f)
            {
                if (!actor.flags.HasFlag(FlagType.OutOfControl))
                {
                    stats.homingTarget = Common.FindHomingTarget();
                    var homingTarget = stats.homingTarget;

                    if (input.JumpPressed)
                    {
                        if (stateMachine.PreviousState is not FStateHoming or FStateAirBoost)
                        {
                            if (homingTarget != null)
                            {
                                stateMachine.SetState<FStateHoming>().SetTarget(homingTarget);
                                stats.homingTarget = null;
                            }
                            else
                            {
                                stateMachine.SetState<FStateHoming>();
                            }
                        }
                    }
                }
            }

            if (!actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (input.BPressed)
                {
                    stateMachine.SetState<FStateStomp>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (!Common.CheckForGround(out _))
            {
                stats.groundNormal = Vector3.up;
                
                Movement(dt);
                Rotate(dt);

                if (stateMachine.PreviousState is not FStateSpecialJump)
                {
                    var drag = 0.65f;
                    var airResistance = 0.95f;
                    var airDrag = new Vector3(
                        -drag * airResistance * dt * _rigidbody.linearVelocity.x,
                        0,
                        -drag * airResistance * dt * _rigidbody.linearVelocity.z
                    );
                    _rigidbody.AddForce(airDrag, ForceMode.VelocityChange);
                }
                
                Common.ApplyGravity(stats.gravity, dt);
            }
            else
            {
                if (stateMachine.GetState<FStateGround>().GetAttachState()) stateMachine.SetState<FStateGround>();
            }
        }

        private void Movement(float dt)
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            var normal = stats.groundNormal;
            SurgeMath.SplitPlanarVector(velocity, normal, out Vector3 planar, out Vector3 airVelocity);
            
            var doc = GameDocument<SonicGameDocument>.GetDocument("Sonic");
            var param = doc.GetGroup(PhysicsGroup);

            stats.movementVector = planar;
            stats.planarVelocity = planar;

            Vector3 transformedInput = Quaternion.FromToRotation(_cameraTransform.up, normal) *
                                       (_cameraTransform.rotation * input.moveVector);
            transformedInput = Vector3.ProjectOnPlane(transformedInput, normal);
            stats.inputDir = transformedInput.normalized * input.moveVector.magnitude;

            if (stats.inputDir.magnitude > 0.2f)
            {
                stats.turnRate = Mathf.Lerp(stats.turnRate, param.GetParameter<float>(BasePhysics_TurnSpeed), dt * param.GetParameter<float>(BasePhysics_TurnSmoothing));
                var accelRateMod = param.GetParameter<AnimationCurve>(BasePhysics_AccelerationCurve).Evaluate(stats.planarVelocity.magnitude / param.GetParameter<float>(BasePhysics_TopSpeed));
                if (stats.planarVelocity.magnitude < param.GetParameter<float>(BasePhysics_TopSpeed))
                    stats.planarVelocity += stats.inputDir * (param.GetParameter<float>(BasePhysics_AccelerationRate) * accelRateMod * dt);
                float handling = stats.turnRate * 0.075f;
                stats.movementVector = Vector3.Lerp(stats.planarVelocity, stats.inputDir.normalized * stats.planarVelocity.magnitude, 
                    dt * handling);
            }

            if (stats.lastContactObject is not TrickJumper) airVelocity = Vector3.ClampMagnitude(airVelocity, param.GetParameter<float>(BasePhysics_MaxVerticalSpeed)); // Trick Jumper fix
            Vector3 movementVelocity = stats.movementVector + airVelocity;
            _rigidbody.linearVelocity = movementVelocity;
        }

        private void Rotate(float dt)
        {
            stats.transformNormal = Vector3.Slerp(stats.transformNormal, Vector3.up, dt * 8f);

            Vector3 vel = _rigidbody.linearVelocity;
            
            if (vel.magnitude > 0.2f)
            {
                vel.y = 0;
                Quaternion rot = Quaternion.LookRotation(vel, stats.transformNormal);
                actor.transform.rotation = rot;
            }
        }

        public void BoostHandle()
        {
            if (!actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (input.BoostPressed && stateMachine.GetSubState<FBoost>().CanBoost())
                {
                    stateMachine.SetState<FStateAirBoost>();
                }
            }
        }

        protected float GetAirTime() => _airTime;

        private void CalculateAirTime(float dt)
        {
            _airTime += dt;
        }
    }
}