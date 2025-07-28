using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    /// <summary>
    /// Trigger for applying and clamping an impulse to the player
    /// </summary>
    public class JumpCollision : ContactBase
    {
        [Header("Properties")] 
        [SerializeField] private float speedMin = 20f;
        [SerializeField, Range(0, 90)] private float pitch = 10f;
        [SerializeField] private bool groundOnly = true;
        [SerializeField, Min(0)] private float impulseOnNormal = 15f;
        [SerializeField, Min(0)] private float impulseOnBoost = 15f;
        [SerializeField] private float outOfControl = 0.5f;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            float dot = Vector3.Dot(context.transform.forward, transform.forward);
            float impulse = SonicTools.IsBoost() ? impulseOnBoost : impulseOnNormal;
            
            if (dot > 0) // Make sure the player is facing the same direction as the jump collision
            {
                if (context.Kinematics.Speed >= speedMin)
                {
                    if (impulse > 0)
                    {
                        var ray = new Ray(context.transform.position, -context.Kinematics.Normal);
                        if (groundOnly && Physics.SphereCast(ray, 0.5f, out _, context.Config.castDistance, context.Config.castLayer) || !groundOnly)
                        {
                            Rigidbody body = context.Kinematics.Rigidbody;
                            body.position += Vector3.up * 0.2f;
                            
                            Vector3 force = Utility.GetImpulseWithPitch(transform.forward, -transform.right, pitch, impulse);
                            context.Kinematics.ResetVelocity();
                            body.AddForce(force, ForceMode.Impulse);
                            body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, impulse);
                            
                            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, outOfControl));
                        }
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, Utility.GetImpulseWithPitch(transform.forward, -transform.right, pitch, impulseOnNormal), Color.green, impulseOnNormal);
            TrajectoryDrawer.DrawTrajectory(transform.position, Utility.GetImpulseWithPitch(transform.forward, -transform.right, pitch, impulseOnBoost), Color.cyan, impulseOnBoost);
        }
    }
}
