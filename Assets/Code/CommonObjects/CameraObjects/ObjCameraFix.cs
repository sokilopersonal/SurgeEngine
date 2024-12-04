using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;

namespace SurgeEngine.Code.CommonObjects
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
            var context = ActorContext.Context;
            context.camera.stateMachine.SetState<FixedCameraPan>(allowSameState: true).SetData(data);
        }
        
        public override void RemovePan()
        {
            var context = ActorContext.Context;
            if (!context.camera.stateMachine.IsExact<NewModernState>())
            {
                context.camera.stateMachine.SetState<RestoreCameraPawn>();
            }
        }
    }
}