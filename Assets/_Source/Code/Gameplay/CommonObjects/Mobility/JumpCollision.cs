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
        [SerializeField] private float speedRequired = 20f;
        [SerializeField, Range(0, 90)] private float pitch = 10f;
        [SerializeField, Min(0)] private float impulseOnNormal = 15f;
        [SerializeField, Min(0)] private float impulseOnBoost = 15f;
        [SerializeField] private float outOfControl = 0.5f;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            float dot = Vector3.Dot(context.transform.forward, context.transform.forward);
            float impulse = SonicTools.IsBoost() ? impulseOnBoost : impulseOnNormal;
            Vector3 cross = Utility.GetCross(transform, pitch);
            
            if (dot > 0) // Make sure the player is facing the same direction as the jump collision
            {
                if (context.Kinematics.Speed >= speedRequired) // If the player is moving fast enough apply an impulse
                {
                    if (impulse > 0)
                    {
                        Rigidbody body = context.Kinematics.Rigidbody;
                        body.linearVelocity = Vector3.zero;
                        context.Stats.planarVelocity = Vector3.zero;
                        context.Stats.movementVector = Vector3.zero;

                        context.transform.position += Vector3.up * 0.25f;
                        context.transform.forward = Utility.GetCross(transform, 0);

                        Vector3 impulseV = cross.normalized * impulse;
                        
                        body.AddForce(impulseV, ForceMode.Impulse);
                        body.linearVelocity = Vector3.ClampMagnitude(body.linearVelocity, impulse);
                    }
                
                    context.Flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, outOfControl));
                }
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            TrajectoryDrawer.DrawTrajectory(transform.position, Utility.GetCross(transform, pitch), Color.green, impulseOnNormal);
            TrajectoryDrawer.DrawTrajectory(transform.position, Utility.GetCross(transform, pitch), Color.cyan, impulseOnBoost);
        }
    }
}
