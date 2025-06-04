using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ObjCameraFix : ObjCameraBase
    {
        public FixPanData data;
        
        private void Awake()
        {
            data.position = transform.position;
            data.target = transform.rotation;
        }
        
        public override void SetPan()
        {
            ActorBase context = ActorContext.Context;
            context.Camera.StateMachine.SetState<FixedCameraPan>(allowSameState: true)?.SetData(data);
        }
        
        public override void RemovePan()
        {
            ActorBase context = ActorContext.Context;
            context.Camera.StateMachine.SetState<RestoreCameraPawn>();
        }
    }
}