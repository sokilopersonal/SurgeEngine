using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjVerticalCameraPan : ObjCameraBase<VerticalCameraPan, VerticalPanData>
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