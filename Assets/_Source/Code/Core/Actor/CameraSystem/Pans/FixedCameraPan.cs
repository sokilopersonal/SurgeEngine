using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class FixedCameraPan : CameraBasePan<FixPanData>
    {
        public FixedCameraPan(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _lastData = _stateMachine.RememberLastData();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            LastCameraData last = _stateMachine.GetLastData();
            
            _stateMachine.Position = Vector3.Lerp(last.position, _panData.position, _stateMachine.interpolatedBlendFactor);
            _stateMachine.Rotation = Quaternion.Lerp(last.rotation, _panData.target, _stateMachine.interpolatedBlendFactor);
            _stateMachine.Camera.fieldOfView = Mathf.Lerp(last.fov, _panData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}