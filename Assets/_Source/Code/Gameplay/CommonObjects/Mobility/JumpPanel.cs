using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using SurgeEngine._Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class JumpPanel : JumpPanelBase
    {
        [SerializeField] private float pitch = 15f;
        private Vector3 StartPosition => transform.position;
        
        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            var body = context.Kinematics.Rigidbody;
            body.position = StartPosition;
            
            Launch(context, pitch);
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, pitch, impulseOnNormal), Color.green, impulseOnNormal);
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, pitch, impulseOnBoost), Color.blue, impulseOnBoost);
        }
    }
}