using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    /// <summary>
    /// Trigger for applying and clamping an impulse to the player
    /// </summary>
    public class JumpCollision : StageObject
    {
        [Header("Properties")] 
        [SerializeField] private float speedMin = 20f;
        [SerializeField, Range(0, 90)] private float pitch = 10f;
        [SerializeField] private bool groundOnly = true;
        [SerializeField, Min(0)] private float impulseOnNormal = 15f;
        [SerializeField, Min(0)] private float impulseOnBoost = 15f;
        [SerializeField] private float outOfControl = 0.5f;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            float dot = Vector3.Dot(context.transform.forward, transform.forward);
            float impulse = impulseOnNormal;
            if (context.StateMachine.GetState(out FBoost boost))
            {
                if (boost.Active)
                    impulse = impulseOnBoost;
            }
            
            if (dot > 0) // Make sure the player is facing the same direction as the jump collision
            {
                if (context.Kinematics.Speed >= speedMin)
                {
                    if (groundOnly && !context.Kinematics.InAir || !groundOnly)
                    {
                        Rigidbody body = context.Kinematics.Rigidbody;
                        body.position += transform.up * 1.5f;

                        Vector3 force = Utility.GetImpulseWithPitch(transform.forward, -transform.right, pitch, impulse);
                        body.linearVelocity = force;
                            
                        context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, outOfControl));
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up, Utility.GetImpulseWithPitch(transform.forward, -transform.right, pitch, impulseOnNormal), Color.green, impulseOnNormal);
            TrajectoryDrawer.DrawTrajectory(transform.position + transform.up, Utility.GetImpulseWithPitch(transform.forward, -transform.right, pitch, impulseOnBoost), Color.cyan, impulseOnBoost);
        }
    }
}
