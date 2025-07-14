using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraFix : ObjCameraBase<FixedCameraPan, FixPanData>
    {
        private void Update()
        {
            data.position = transform.position;
            data.target = transform.rotation;
        }
    }
}