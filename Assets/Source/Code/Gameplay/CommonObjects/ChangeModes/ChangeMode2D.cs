using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeMode2D : ModeCollision
    {
        [SerializeField] private float pathEaseTime = 1f;

        protected override void SetMode(CharacterBase ctx)
        {
            ctx.Kinematics.Mode.Set2DMode(new ChangeMode2DData(ctx.transform.position, isChangeCamera, pathEaseTime));
        }

        protected override void RemoveMode(CharacterBase ctx)
        {
            ctx.Kinematics.Mode.Set2DMode(null);
        }

        public override void ImportSetData(string objectName, XElement elem)
        {
            base.ImportSetData(objectName, elem);
            
            pathEaseTime = HE1Helper.GetFloat(elem, "m_PathEaseTime");
        }
    }

    public class ChangeMode2DData : ChangeModeData
    {
        public Vector3 StartPosition { get; set; }
        public float PathEaseTime { get; private set; }
        public float CurrentEaseTime { get; set; }

        public ChangeMode2DData(Vector3 startPos, bool isCameraChange, float pathEaseTime) : base(isCameraChange)
        {
            StartPosition = startPos;
            PathEaseTime = pathEaseTime;
        }
    }
}