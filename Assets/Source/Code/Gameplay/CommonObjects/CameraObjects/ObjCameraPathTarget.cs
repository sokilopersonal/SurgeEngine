using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.CameraSystem;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Tools;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraPathTarget : ObjCameraBase<PathTargetCameraPan, PathPanData>
    {
        public override void SetPan(CharacterBase ctx, CameraEaseData? easeOverride = null)
        {
            if (data.container != null)
            {
                base.SetPan(ctx, easeOverride);
            }
        }

        public override void RemovePan(CharacterBase ctx)
        {
            if (data.container != null)
            {
                base.RemovePan(ctx);
            }
        }

        public override void ImportSetData(string objectName, XElement elem)
        {
            base.ImportSetData(objectName, elem);
            
            float offsetOnEyePath = HE1Helper.GetFloat(elem, "OffsetOnEyePath");
            data.offsetOnEye = offsetOnEyePath;
        }
    }
}