using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns.Data;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class FixedCameraPan : CState
    {
        private FixPanData _fData;
        
        public FixedCameraPan(Actor owner) : base(owner)
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
            
            _fData = (FixPanData)_data;
            _stateMachine.currentData = _fData;
            
            LastCameraData last = _stateMachine.GetLastData();
            
            _stateMachine.position = Vector3.Lerp(last.position, _fData.position, _stateMachine.interpolatedBlendFactor);
            _stateMachine.rotation = Quaternion.Slerp(last.rotation, _fData.target, _stateMachine.interpolatedBlendFactor);
            _stateMachine.camera.fieldOfView = Mathf.Lerp(last.fov, _fData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}