using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SonicSubStates.Boost;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateAir : FStateMove, IBoostHandler
    {
        private float _airTime;

        public FStateAir(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _airTime = 0f;
            
            Kinematics.Normal = Vector3.up;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            CalculateAirTime(dt);
            
            if (GetAirTime() > 0.1f)
            {
                if (!Actor.flags.HasFlag(FlagType.OutOfControl))
                {
                    var homingTarget = Stats.homingTarget;

                    if (Input.JumpPressed)
                    {
                        if (StateMachine.PreviousState is not FStateHoming or FStateAirBoost)
                        {
                            if (homingTarget != null)
                            {
                                StateMachine.SetState<FStateHoming>().SetTarget(homingTarget);
                            }
                            else
                            {
                                StateMachine.SetState<FStateHoming>();
                            }
                        }
                    }
                }
            }

            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.BPressed)
                {
                    StateMachine.SetState<FStateStomp>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (!Common.CheckForGround(out _))
            {
                Kinematics.Normal = Vector3.up;
                
                Actor.kinematics.BasePhysics(Vector3.zero, Vector3.up);
                
                // TODO: Use Actor.model.RotateBody
                Rotate(dt);

                // TODO: Move to ActorKinematics
                if (StateMachine.PreviousState is not FStateSpecialJump or FStateAirBoost)
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
                
                Common.ApplyGravity(Stats.gravity, Time.fixedDeltaTime);
            }
            else
            {
                if (Kinematics.GetAttachState()) StateMachine.SetState<FStateGround>();
            }
        }

        private void Rotate(float dt)
        {
            Stats.transformNormal = Vector3.Slerp(Stats.transformNormal, Vector3.up, dt * 8f);

            Vector3 vel = _rigidbody.linearVelocity;
            vel.y = 0;
            
            if (vel.magnitude > 0.5f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, Stats.transformNormal);
                Actor.transform.rotation = rot;
            }
        }

        public void BoostHandle()
        {
            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                FBoost boost = StateMachine.GetSubState<FBoost>();
                if (Input.BoostPressed && boost.CanBoost())
                {
                    StateMachine.SetState<FStateAirBoost>();
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