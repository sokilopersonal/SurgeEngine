using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateAir : FStateMove, IBoostHandler, IDamageableState
    {
        protected float _airTime;

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
                    HomingTarget homingTarget = Stats.homingTarget;

                    if (Input.JumpPressed)
                    {
                        if (StateMachine.PreviousState is not FStateHoming or FStateAirBoost)
                        {
                            if (homingTarget != null)
                            {
                                StateMachine.SetState<FStateHoming>()?.SetTarget(homingTarget);
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
                
                Actor.kinematics.BasePhysics(Vector3.zero, Vector3.up, MovementType.Air);
                
                Vector3 vel = Actor.kinematics.Velocity;
                vel.y = 0;
                Model.RotateBody(vel, Vector3.up);
                
                Common.ApplyGravity(Stats.gravity, Time.fixedDeltaTime);
            }
            else
            {
                if (Kinematics.GetAttachState())
                {
                    if (Kinematics.HorizontalSpeed > 5f) StateMachine.SetState<FStateGround>();
                    else StateMachine.SetState<FStateIdle>();
                }
            }
        }

        public void BoostHandle()
        {
            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                FBoost boost = StateMachine.GetSubState<FBoost>();
                if (Input.BoostPressed && boost.CanBoost() && boost.canAirBoost)
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