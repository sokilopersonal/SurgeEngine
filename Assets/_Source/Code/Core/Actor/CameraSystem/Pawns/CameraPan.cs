using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns
{
    public class CameraPan : CameraState<PanData>
    {
        public CameraPan(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _lastData = _stateMachine.RememberLastData();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _stateMachine.SetDirection(_stateMachine.transform.transform.forward);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _stateMachine.position = Vector3.Lerp(_lastData.position, _panData.position, _stateMachine.interpolatedBlendFactor);
            
            Quaternion rotation = Quaternion.LookRotation(_actor.transform.position + _stateMachine.transform.TransformDirection(_stateMachine.lookOffset) - _stateMachine.position, Vector3.up);
            _stateMachine.rotation = Quaternion.Lerp(_lastData.rotation, rotation, _stateMachine.interpolatedBlendFactor);
            _stateMachine.fov = Mathf.Lerp(_lastData.fov, _panData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}