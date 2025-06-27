using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraPoint : ObjCameraBase<PointCameraPan, PointPanData>
    {
        private void Awake()
        {
            data.Forward = transform.forward;
        }
    }
}