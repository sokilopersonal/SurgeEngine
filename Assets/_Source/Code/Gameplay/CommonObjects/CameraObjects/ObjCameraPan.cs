using System;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;

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