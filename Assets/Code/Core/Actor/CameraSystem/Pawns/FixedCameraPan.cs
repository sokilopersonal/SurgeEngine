using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns
{
    public class FixedCameraPan : CameraState<FixPanData>
    {
        public FixedCameraPan(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stateMachine.ResetBlendFactor();
            _lastData = _stateMachine.RememberLastData();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            LastCameraData last = _stateMachine.GetLastData();
            
            _stateMachine.position = Vector3.Lerp(last.position, _panData.position, _stateMachine.interpolatedBlendFactor);
            _stateMachine.rotation = Quaternion.Slerp(last.rotation, _panData.target, _stateMachine.interpolatedBlendFactor);
            _stateMachine.camera.fieldOfView = Mathf.Lerp(last.fov, _panData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}