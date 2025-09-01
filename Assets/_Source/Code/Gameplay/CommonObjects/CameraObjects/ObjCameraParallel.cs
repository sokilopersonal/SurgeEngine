using SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans.Data;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraParallel : ObjCameraBase<ParallelCameraPan, ParallelPanData>
    {
        private void Awake()
        {
            data.position = transform.position;
            data.forward = transform.forward;
        }

        private void Update()
        {
            data.forward = transform.forward;
        }
    }
}