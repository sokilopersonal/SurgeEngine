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
            _lastData = _stateMachine.RememberLastData();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _stateMachine.SetDirection(_vData.RestoreDirection());
        }

        public override void OnTick(float dt)
        {
            _vData = (VerticalPanData)_data;
            _stateMachine.currentData = _vData;
            _distance = _vData.distance;
            _yOffset = _vData.yOffset;
            
            _stateMachine.SetDirection(_vData.forward);
            
            _stateMachine.camera.fieldOfView = Mathf.Lerp(_lastData.fov, _vData.fov, _stateMachine.interpolatedBlendFactor);
            
            base.OnTick(dt);
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            _stateMachine.position = Vector3.Lerp(_lastData.position, targetPosition, _stateMachine.interpolatedBlendFactor);
        }
        
        protected override void SetRotation(Vector3 actorPosition)
        {
            Quaternion rot = Quaternion.LookRotation(actorPosition + _stateMachine.transform.TransformDirection(_stateMachine.lookOffset) - _stateMachine.position, Vector3.up);

            if (rot == new Quaternion(0, 0, 0, 1))
            {
                rot = Quaternion.identity;
            }
            
            _stateMachine.rotation = Quaternion.Lerp(_lastData.rotation, rot, _stateMachine.interpolatedBlendFactor);
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