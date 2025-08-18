using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateAir : FCharacterState, IDamageableState, IPointMarkerLoader
    {
        public float AirTime { get; private set; }

        public bool IsFallDeath { get; set; }

        public FStateAir(CharacterBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (Mathf.Abs(Kinematics.Angle - 90) < 0.05f && Kinematics.Velocity.y > 3f)
            {
                character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, 0.5f));
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            bool air = !Kinematics.CheckForGround(out var hit);
            bool isWater = false;
            if (hit.transform != null)
            {
                isWater = hit.transform.gameObject.GetGroundTag() == GroundTag.Water;
            }

            if (isWater)
            {
                if (hit.transform.TryGetComponent(out WaterSurface water) && Kinematics.HorizontalVelocity.magnitude > water.MinimumSpeed && Kinematics.VerticalVelocity.y < 0)
                {
                    StateMachine.SetState<FStateGround>();
                    return;
                }
            }
            
            if (air || isWater)
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;
                
                Kinematics.BasePhysics(Vector3.up, MovementType.Air);
                
                Vector3 vel = Kinematics.Velocity;
                vel.y = 0;
                Model.RotateBody(vel, Vector3.up, 360f);
                
                float gravity = Kinematics.Gravity;
                if (character.Flags.HasFlag(FlagType.OnWater))
                {
                    gravity /= 4f;
                }
                
                Kinematics.ApplyGravity(gravity);

                if (Kinematics.mode == KinematicsMode.Forward)
                {
                    var path = Kinematics.GetPath();
                    var pos = path.EvaluatePosition();
                    
                    var ray = new Ray(Rigidbody.position, pos - Rigidbody.position);
                    if (Physics.Raycast(ray, out var predictHit, 1.5f, character.Config.castLayer, QueryTriggerInteraction.Ignore))
                    {
                        Kinematics.Normal = predictHit.normal;
                        Kinematics.Snap(predictHit.point, predictHit.normal);
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
                        if (speed > character.Config.landingSpeed) StateMachine.SetState<FStateGround>();
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