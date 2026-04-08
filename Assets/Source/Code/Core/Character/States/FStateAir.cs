using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateAir : FCharacterState, IDamageableState, IWallJumpDetect
    {
        public bool WallDetected { get; set; }

        public FStateAir(CharacterBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (Mathf.Abs(Kinematics.Angle - 90) < 0.05f && Kinematics.Velocity.y > 3f)
            {
                Character.Flags.AddFlag(new Flag(FlagType.OutOfControl, 0.5f));
            }
            
            Kinematics.Normal = Vector3.up;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Vector3 vel = Kinematics.Velocity;
            vel.y = 0;
            Model.RotateBody(vel, Vector3.up, 4f);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            bool air = !Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down, 1f);
            bool isWater = hit.transform.IsWater(out var surface);
            if (isWater)
            {
                if (Kinematics.HorizontalVelocity.magnitude > surface.MinimumSpeed 
                    && Kinematics.VerticalVelocity.y < 0)
                {
                    StateMachine.SetState<FStateGround>();
                    return;
                }
            }
            
            float gravity = Kinematics.Gravity;
            if (Character.Flags.HasFlag(FlagType.OnWater))
            {
                gravity /= 4f;
            }
            Kinematics.ApplyGravity(gravity);
            
            if (air || isWater)
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;
                
                Kinematics.BasePhysics(Vector3.up, MovementType.Air);
                
                /*if (Kinematics.Path2D != null && Kinematics.GetAttachState()) // fix it in the future??
                {
                    var path = Kinematics.Path2D.Spline;
                    var pos = path.EvaluatePosition();
                    var up = path.EvaluateUp();
                    
                    var ray = new Ray(Rigidbody.position, pos - Rigidbody.position);
                    if (Physics.Raycast(ray, out var predictHit, 1f, Character.Config.castLayer, QueryTriggerInteraction.Ignore))
                    {
                        Kinematics.Normal = predictHit.normal;
                        Kinematics.Snap(pos + up);
                        Rigidbody.rotation = Quaternion.FromToRotation(Vector3.up, up) * Rigidbody.rotation;
                        Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, up);
                    }
                }*/
            }
            else
            {
                bool predictedGround = Kinematics.CheckForPredictedGround(dt, Character.Config.castDistance, 4);
                if (Kinematics.GetAttachState())
                {
                    var vel = Kinematics.Velocity;
                    vel.y = 0;
                    float speed = vel.magnitude;
                    if (speed > Character.Config.landingSpeed)
                    {
                        StateMachine.SetState<FStateGround>();
                    }
                    else
                    {
                        if (Kinematics.GetInputDir().magnitude < 0.1f)
                        {
                            StateMachine.SetState<FStateIdle>();
                        }
                        else
                        {
                            StateMachine.SetState<FStateGround>();
                        }
                    }
                }
            }
        }
    }
}