using SurgeEngine.Code.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Actor.System;

namespace SurgeEngine.Code.CommonObjects.CameraObjects
{
    public class ObjVerticalCameraPan : ObjCameraBase
    {
        public VerticalPanData data;

        private void Awake()
        {
            data.position = transform.position;
            data.forward = transform.forward;
        }

        private void Update()
        {
            data.forward = transform.forward;
        }

        public override void SetPan()
        {
            ActorBase context = ActorContext.Context;
            context.camera.stateMachine.SetState<VerticalCameraPan>(allowSameState: true).SetData(data);
        }
        
        public override void RemovePan()
        {
            ActorBase context = ActorContext.Context;
            context.camera.stateMachine.SetState<RestoreCameraPawn>(allowSameState: true);
        }
    }
}