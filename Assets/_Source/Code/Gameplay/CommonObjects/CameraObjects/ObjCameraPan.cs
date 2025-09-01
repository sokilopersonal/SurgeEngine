using SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans.Data;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraPan : ObjCameraBase<CameraPan, PanData>
    {
        private void Awake()
        {
            data.position = transform.position;
        }
    }
}