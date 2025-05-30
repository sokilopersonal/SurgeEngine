using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraNormal : ObjCameraBase
    {
        public NormalPanData data;
        
        public override void SetPan()
        {
            ActorBase context = ActorContext.Context;
            context.Camera.stateMachine.SetState<NormalCameraPan>(allowSameState: true)?.SetData(data);
        }

        public override void RemovePan()
        {
            ActorBase context = ActorContext.Context;
            context.Camera.stateMachine.SetState<RestoreCameraPawn>();
        }
    }
}