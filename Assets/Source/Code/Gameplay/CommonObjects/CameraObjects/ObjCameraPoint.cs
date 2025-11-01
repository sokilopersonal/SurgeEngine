using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraPoint : ObjCameraBase<PointCameraPan, PointPanData>
    {
        private void Awake()
        {
            if (data.target == null)
            {
                data.target = transform;
            }
        }
    }
}