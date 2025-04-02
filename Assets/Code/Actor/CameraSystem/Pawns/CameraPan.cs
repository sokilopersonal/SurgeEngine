using SurgeEngine.Code.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Actor.CameraSystem.Pawns
{
    public class CameraPan : CameraPawn
    {
        private PanData _panData;
        private LastCameraData _lastData;
        
        public CameraPan(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _lastData = _stateMachine.RememberLastData();
            
            _stateMachine.ResetBlendFactor();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _stateMachine.SetDirection(_panData.RestoreDirection());
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _panData = (PanData)_data;
            _stateMachine.position = Vector3.Lerp(_lastData.position, _panData.position, _stateMachine.interpolatedBlendFactor);
            
            Quaternion rotation = Quaternion.LookRotation(_actor.transform.position + _stateMachine.transform.TransformDirection(_stateMachine.lookOffset) - _stateMachine.position, Vector3.up);
            _stateMachine.rotation = Quaternion.Lerp(_lastData.rotation, rotation, _stateMachine.interpolatedBlendFactor);
            _stateMachine.camera.fieldOfView = Mathf.Lerp(_lastData.fov, _panData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}