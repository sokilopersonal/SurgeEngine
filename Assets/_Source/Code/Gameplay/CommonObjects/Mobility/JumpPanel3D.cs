using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class JumpPanel3D : JumpPanelBase
    {
        private Vector3 StartPosition => transform.position + Vector3.up;
        private float Pitch => 36f;
        
        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            Launch(context, Pitch);
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, Pitch, impulse), Color.green, impulse);
        }
    }
}