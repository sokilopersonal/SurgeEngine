using SurgeEngine.Code.ActorSystem;
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
            _stateMachine.position = Vector3.Slerp(_lastData.position, diff, _stateMachine.interpolatedBlendFactor);
            _stateMachine.position += _actor.transform.position;
        }
        
        protected override void SetRotation(Vector3 actorPosition)
        {
            var rot = Quaternion.LookRotation(actorPosition + _stateMachine.transform.TransformDirection(_stateMachine.lookOffset) - _stateMachine.position, Vector3.up);

            if (rot == new Quaternion(0, 0, 0, 1))
            {
                rot = Quaternion.identity;
            }
            
            _stateMachine.rotation = Quaternion.Slerp(_lastData.rotation, rot, _stateMachine.interpolatedBlendFactor);
        }

        protected override void LookAxis()
        {
            
        }

        protected override void AutoLookDirection()
        {
            _stateMachine.xAutoLook = 0;
            _stateMachine.yAutoLook = 0;
        }
    }
}