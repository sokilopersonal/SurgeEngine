using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class FixedCameraPan : CState
    {
        private FixPanData _fData;
        
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private float _lastFOV;
        
        public FixedCameraPan(Actor owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stateMachine.ResetBlendFactor();
            
            _lastPosition = _stateMachine.position;
            _lastRotation = _stateMachine.rotation;
            _lastFOV = _stateMachine.camera.fieldOfView;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _fData = (FixPanData)_data;
            _stateMachine.currentData = _fData;
            
            _stateMachine.position = Vector3.Lerp(_lastPosition, _fData.position, _stateMachine.interpolatedBlendFactor);
            _stateMachine.rotation = Quaternion.Slerp(_lastRotation, _fData.target, _stateMachine.interpolatedBlendFactor);
            _stateMachine.camera.fieldOfView = Mathf.Lerp(_lastFOV, _fData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}