using SurgeEngine.Code.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Actor.System;

namespace SurgeEngine.Code.CommonObjects.CameraObjects
{
    public class ObjCameraPan : ObjCameraBase
    {
        public PanData data;

        private void Awake()
        {
            data.position = transform.position;
        }
        
        public override void SetPan()
        {
            ActorBase context = ActorContext.Context;
            context.camera.stateMachine.SetState<CameraPan>(allowSameState: true).SetData(data);
        }
        
        public override void RemovePan()
        {
            ActorBase context = ActorContext.Context;
            context.camera.stateMachine.SetState<RestoreCameraPawn>();
        }
    }
}