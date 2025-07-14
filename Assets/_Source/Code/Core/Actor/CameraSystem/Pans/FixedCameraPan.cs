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
            
            StatePosition = _panData.position;
            StateRotation = _panData.target;
            StateFOV = _panData.fov;
        }
    }
}