using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class JumpPanel : JumpPanelBase
    {
        [SerializeField] private float pitch = 15f;
        protected override Vector3 StartPosition => transform.position + transform.up * Mathf.Max(transform.localScale.y, 1f);

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            Launch(context, pitch);
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, pitch, impulseOnNormal), Color.green, impulseOnNormal);
            TrajectoryDrawer.DrawTrajectory(StartPosition, Utility.GetImpulseWithPitch(-transform.forward, transform.right, pitch, impulseOnBoost), Color.blue, impulseOnBoost);
        }

        public override void ImportSetData(string objectName, XElement elem)
        {
            base.ImportSetData(objectName, elem);
            
            int angleType = (int)HE1Helper.GetFloat(elem, "AngleType");
            pitch = angleType == 0 ? 15 : angleType == 1 ? 30 : angleType > 1 ? angleType : 0;
        }
    }
}