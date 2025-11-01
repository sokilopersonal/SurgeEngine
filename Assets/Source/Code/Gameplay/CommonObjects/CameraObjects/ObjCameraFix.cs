using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects
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