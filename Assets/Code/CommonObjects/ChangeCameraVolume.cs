using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ChangeCameraVolume : ContactBase
    {
        [SerializeField] private ObjectCameraPan target;
        
        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            SetPan();
        }

        private void OnTriggerExit(Collider other)
        {
            if (ActorContext.Context.gameObject == other.transform.parent.gameObject)
            {
                RemovePan();
            }
        }

        public void SetPan()
        {
            var context = ActorContext.Context;
            context.camera.stateMachine.SetState<CameraPan>().SetData(target.data);
        }
        
        public void RemovePan()
        {
            var context = ActorContext.Context;
            context.camera.stateMachine.SetState<RestoreCameraPawn>().SetData(target.data);
        }
    }
}