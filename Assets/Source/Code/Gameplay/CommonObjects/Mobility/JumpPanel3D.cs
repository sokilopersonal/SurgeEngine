using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class JumpPanel3D : JumpPanelBase
    {
        private Vector3 StartPosition => transform.position + transform.up * Mathf.Max(transform.localScale.y, 1f);
        private const float Pitch = 30f;
        
        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            var body = context.Kinematics.Rigidbody;
            body.position = StartPosition;
            
            Launch(context, Pitch);
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, Pitch, impulseOnNormal), Color.green, impulseOnNormal);
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, Pitch, impulseOnBoost), Color.blue, impulseOnBoost);
        }
    }
}