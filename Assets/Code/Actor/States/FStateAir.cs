using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.States.SonicSubStates;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States
{
    public class FStateAir : FStateMove, IBoostHandler, IDamageableState
    {
        protected float AirTime;

        public FStateAir(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (Actor.kinematics.Angle >= 90 && Actor.kinematics.Velocity.y > 3f)
            {
                Actor.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, 0.5f));
            }
            
            AirTime = 0f;
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
                
                float gravity = Stats.gravity;
                if (Actor.flags.HasFlag(FlagType.OnWater))
                {
                    gravity /= 4f;
                }
                
                Common.ApplyGravity(gravity, Time.fixedDeltaTime);
            }
            else
            {
                if (Kinematics.GetAttachState())
                {
                    if (Kinematics.Speed > 5f) StateMachine.SetState<FStateGround>();
                    else StateMachine.SetState<FStateIdle>();
                }
            }
        }

        public void BoostHandle()
        {
            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                FBoost boost = StateMachine.GetSubState<FBoost>();
                if (Input.BoostPressed && boost.CanBoost() && boost.CanAirBoost)
                {
                    StateMachine.SetState<FStateAirBoost>();
                }
            }
        }

        protected float GetAirTime() => AirTime;

        private void CalculateAirTime(float dt)
        {
            AirTime += dt;
        }
    }
}