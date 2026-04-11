using System.Collections.Generic;
using System.Xml.Linq;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine.Source.Code.Tools;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraPoint : ObjCameraBase<PointCameraPan, PointPanData>, IHE1TargetResolvable
    {
        private void Awake()
        {
            if (data.target == null)
            {
                data.target = transform;
            }
        }

        public void ResolveTarget(XElement elem, Dictionary<long, StageObject> sceneObjects)
        {
            if (sceneObjects.TryGetValue(HE1Helper.GetTargetID(elem), out var target))
            {
                data.target = target.transform;
            }
        }
    }
}