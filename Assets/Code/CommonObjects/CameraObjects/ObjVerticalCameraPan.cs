using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;

namespace SurgeEngine.Code.CommonObjects
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
            var context = ActorContext.Context;
            context.camera.stateMachine.SetState<VerticalCameraPan>(allowSameState: true).SetData(data);
        }
        
        public override void RemovePan()
        {
            var context = ActorContext.Context;
            context.camera.stateMachine.SetState<RestoreCameraPawn>();
        }
    }
}