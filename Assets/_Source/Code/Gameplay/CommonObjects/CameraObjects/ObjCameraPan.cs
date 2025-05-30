using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
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
            context.Camera.stateMachine.SetState<CameraPan>(allowSameState: true).SetData(data);
        }
        
        public override void RemovePan()
        {
            ActorBase context = ActorContext.Context;
            context.Camera.stateMachine.SetState<RestoreCameraPawn>();
        }
    }
}