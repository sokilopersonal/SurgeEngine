using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class VerticalCameraPan : NewModernState
    {
        private VerticalPanData _vData;
        
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        
        public VerticalCameraPan(Actor owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stateMachine.ResetBlendFactor();
            _lastPosition = _stateMachine.position;
            _lastRotation = _stateMachine.rotation;
        }

        public override void OnTick(float dt)
        {
            _vData = (VerticalPanData)_data;
            _stateMachine.currentData = _vData;
            _distance = _vData.groundOffset;
            _yOffset = _vData.yOffset;
            
            SetDirection(_vData.forward);
            
            base.OnTick(dt);
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            _stateMachine.position = Vector3.Lerp(_lastPosition, targetPosition, _stateMachine.interpolatedBlendFactor);
        }
        
        protected override void SetRotation(Vector3 actorPosition)
        {
            _stateMachine.rotation = Quaternion.Slerp(_lastRotation, 
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