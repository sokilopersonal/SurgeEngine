using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class JumpPanel : JumpPanelBase
    {
        [SerializeField] private float pitch = 15f;
        private Vector3 StartPosition => transform.position;
        
        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            Launch(context, pitch);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, pitch, impulse), Color.green, impulse);
        }

    }
}