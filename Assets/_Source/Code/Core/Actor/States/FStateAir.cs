using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateAir : FActorState, IDamageableState, IPointMarkerLoader
    {
        public float AirTime { get; private set; }

        public bool IsFallDeath { get; set; }

        public FStateAir(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (Mathf.Abs(Kinematics.Angle - 90) < 0.05f && Kinematics.Velocity.y > 3f)
            {
                Actor.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, 0.5f));
            }
            
            Kinematics.Normal = Vector3.up;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            bool air = !Kinematics.CheckForGround(out var hit);
            if (air)
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;
                
                Kinematics.BasePhysics(Vector3.up, MovementType.Air);
                
                Vector3 vel = Kinematics.Velocity;
                vel.y = 0;
                Model.RotateBody(vel, Vector3.up, 360f);
                
                float gravity = Kinematics.Gravity;
                if (Actor.Flags.HasFlag(FlagType.OnWater))
                {
                    gravity /= 4f;
                }
                
                Kinematics.ApplyGravity(gravity);
            }
            else
            {
                if (Kinematics.GetAttachState())
                {
                    if (!Kinematics.IsHardAngle(hit.normal))
                    {
                        StateMachine.SetState<FStateGround>();
                    }
                    else
                    {
                        StateMachine.SetState<FStateSlip>();
                    }
                }
            }
        }

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            IsFallDeath = false;
        }
    }
}