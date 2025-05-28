using System;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateAir : FStateMove, IBoostHandler, IDamageableState, IPointMarkerLoader
    {
        protected float AirTime;

        public bool IsFallDeath { get; set; }

        public FStateAir(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (Mathf.Abs(Kinematics.Angle - 90) < 0.05f && Actor.kinematics.Velocity.y > 3f)
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
            
            if (!Kinematics.CheckForGround(out var hit))
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;
                
                Kinematics.BasePhysics(Vector3.up, MovementType.Air);
                
                Vector3 vel = Kinematics.Velocity;
                vel.y = 0;
                Model.RotateBody(vel, Vector3.up);
                
                float gravity = Stats.gravity;
                if (Actor.flags.HasFlag(FlagType.OnWater))
                {
                    gravity /= 4f;
                }
                
                Kinematics.ApplyGravity(gravity);
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

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            IsFallDeath = false;
        }
    }
}