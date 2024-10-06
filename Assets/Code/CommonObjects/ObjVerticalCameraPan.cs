using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ObjVerticalCameraPan : ObjCameraBase
    {
        public VerticalPanData data;

        private void Awake()
        {
            data.position = transform.position;
        }

        public override void SetPan()
        {
            var context = ActorContext.Context;
            context.camera.stateMachine.SetState<VerticalCameraPan>().SetData(data);
        }
        
        public override void RemovePan()
        {
            var context = ActorContext.Context;
            context.camera.stateMachine.SetState<RestoreCameraPawn>().SetData(data);
        }
    }
}