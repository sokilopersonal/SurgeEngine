using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraPan : ObjCameraBase<CameraPan, PanData>
    {
        private void Awake()
        {
            data.position = transform.position;
        }
    }
}