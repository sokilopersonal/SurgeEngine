using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
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

            bool air = !Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down);
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
                
                if (Kinematics.Path2D != null && Kinematics.GetAttachState())
                {
                    var path = Kinematics.Path2D.Spline;
                    var pos = path.EvaluatePosition();
                    var up = path.EvaluateUp();
                    
                    var ray = new Ray(Rigidbody.position, pos - Rigidbody.position);
                    if (Physics.Raycast(ray, out var predictHit, 1.5f, character.Config.castLayer, QueryTriggerInteraction.Ignore))
                    {
                        Kinematics.Normal = predictHit.normal;
                        Kinematics.Snap(pos + up);
                        Rigidbody.rotation = Quaternion.FromToRotation(Vector3.up, up) * Rigidbody.rotation;
                        Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, up);
                        StateMachine.SetState<FStateGround>();
                    }
                }
            }
            else
            {
                bool predictedGround = Kinematics.CheckForPredictedGround(dt, character.Config.castDistance, 4);
                if (Kinematics.GetAttachState() && predictedGround)
                {
                    float speed = Kinematics.HorizontalVelocity.magnitude;
                    if (speed > character.Config.landingSpeed) StateMachine.SetState<FStateGround>();
                    else StateMachine.SetState<FStateIdle>();
                }
            }
            
            float gravity = Kinematics.Gravity;
            if (character.Flags.HasFlag(FlagType.OnWater))
            {
                gravity /= 4f;
            }
            Kinematics.ApplyGravity(gravity);
        }

        public void Load()
        {
            IsFallDeath = false;
        }
    }
}