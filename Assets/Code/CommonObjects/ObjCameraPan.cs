using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;

namespace SurgeEngine.Code.CommonObjects
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
            var context = ActorContext.Context;
            context.camera.stateMachine.SetState<CameraPan>(allowSameState: true).SetData(data);
        }
        
        public override void RemovePan()
        {
            var context = ActorContext.Context;
            if (!context.camera.stateMachine.Is<DefaultModernPawn>())
            {
                context.camera.stateMachine.SetState<RestoreCameraPawn>();
            }
        }
    }
}