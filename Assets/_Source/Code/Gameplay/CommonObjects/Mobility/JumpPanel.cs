using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using SurgeEngine._Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class JumpPanel : JumpPanelBase
    {
        [SerializeField] private float pitch = 15f;
        private Vector3 StartPosition => transform.position + transform.up;
        
        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            context.Rigidbody.position = StartPosition;
            
            Launch(context, pitch);
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, pitch, impulse), Color.green, impulse);
        }
    }
}