using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class CameraPan : CameraPawn
    {
        private PanData _panData;
        
        public CameraPan(Actor owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stateMachine.ResetBlendFactor();
            _stateMachine.RememberLastData();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _panData = (PanData)_data;

            var last = _stateMachine.GetLastData();
            
            Transform cam = _stateMachine.transform;
            
            Vector3 center = _actor.transform.position;
            Vector3 camCenter = last.position - center;
            Vector3 targetCenter = _panData.position - center;
            
            _stateMachine.position = Vector3.Slerp(camCenter, targetCenter, _stateMachine.interpolatedBlendFactor);
            _stateMachine.position += center;
            
            //_stateMachine.position = Vector3.Slerp(last.position, _panData.position, _stateMachine.interpolatedBlendFactor);
            var rotation = Quaternion.LookRotation(_actor.transform.position - cam.position, Vector3.up);
            _stateMachine.rotation = Quaternion.Slerp(last.rotation, rotation, _stateMachine.interpolatedBlendFactor);
            _stateMachine.camera.fieldOfView = Mathf.Lerp(last.fov, _panData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}