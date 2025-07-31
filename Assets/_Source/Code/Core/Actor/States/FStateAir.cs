using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Infrastructure.Custom;
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

                if (Kinematics.mode == KinematicsMode.Forward)
                {
                    var path = Kinematics.GetPath();
                    var pos = path.EvaluatePosition();
                    
                    var ray = new Ray(Rigidbody.position, pos - Rigidbody.position);
                    if (Physics.Raycast(ray, out var predictHit, 1.5f, Actor.Config.castLayer, QueryTriggerInteraction.Ignore))
                    {
                        Kinematics.Normal = predictHit.normal;
                        Kinematics.Snap(predictHit.point, predictHit.normal, true);
                        Rigidbody.rotation = Quaternion.FromToRotation(Vector3.up, predictHit.normal) * Rigidbody.rotation;
                        StateMachine.SetState<FStateGround>();
                    }
                }
            }
            else
            {
                if (Kinematics.GetAttachState())
                {
                    if (!Kinematics.IsHardAngle(hit.normal))
                    {
                        float speed = Kinematics.HorizontalVelocity.magnitude;
                        if (speed > Actor.Config.landingSpeed) StateMachine.SetState<FStateGround>();
                        else StateMachine.SetState<FStateIdle>();
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