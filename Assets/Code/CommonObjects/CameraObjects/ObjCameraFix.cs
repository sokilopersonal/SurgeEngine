using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;
using SurgeEngine.Code.CameraSystem.Pawns.Data;

namespace SurgeEngine.Code.CommonObjects.CameraObjects
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
            Actor context = ActorContext.Context;
            context.camera.stateMachine.SetState<FixedCameraPan>(allowSameState: true).SetData(data);
        }
        
        public override void RemovePan()
        {
            Actor context = ActorContext.Context;
            if (!context.camera.stateMachine.IsExact<NewModernState>())
            {
                context.camera.stateMachine.SetState<RestoreCameraPawn>();
            }
        }
    }
}