using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class VerticalCameraPan : NewModernState
    {
        private VerticalPanData _vData;
        private LastCameraData _lastData;
        
        public VerticalCameraPan(Actor owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            _stateMachine.ResetBlendFactor();
            _stateMachine.RememberRelativeLastData();
            
            _lastData = _stateMachine.GetLastData();
        }

        public override void OnTick(float dt)
        {
            _vData = (VerticalPanData)_data;
            _stateMachine.currentData = _vData;
            _distance = _vData.groundOffset;
            _yOffset = _vData.yOffset;
            
            SetDirection(_vData.forward);
            
            _stateMachine.camera.fieldOfView = Mathf.Lerp(_lastData.fov, _vData.fov, _stateMachine.interpolatedBlendFactor);
            
            base.OnTick(dt);
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            Vector3 diff = targetPosition - _actor.transform.position;
            _stateMachine.position = Vector3.Lerp(_lastData.position, diff, _stateMachine.interpolatedBlendFactor);
            _stateMachine.position += _actor.transform.position;
        }
        
        protected override void SetRotation(Vector3 actorPosition)
        {
            _stateMachine.rotation = Quaternion.Lerp(_lastData.rotation, 
                Quaternion.LookRotation(actorPosition - _stateMachine.position, Vector3.up),
                _stateMachine.interpolatedBlendFactor);
        }

        protected override void AutoLookDirection()
        {
            _stateMachine.xAutoLook = 0;
            _stateMachine.yAutoLook = 0;
        }
    }
}