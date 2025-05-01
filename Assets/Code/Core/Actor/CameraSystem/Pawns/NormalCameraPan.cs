using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns
{
    public class NormalCameraPan : NewModernState
    {
        private NormalPanData _nData => _panData as NormalPanData;
        
        public NormalCameraPan(ActorBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _lastData = _stateMachine.RememberRelativeLastData();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.distance = Mathf.Lerp(_lastData.distance, _nData.distance, _stateMachine.interpolatedBlendFactor);
            _stateMachine.yOffset = Mathf.Lerp(_lastData.yOffset, _nData.yOffset, _stateMachine.interpolatedBlendFactor);
            
            _stateMachine.fov = Mathf.Lerp(_lastData.fov, _nData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}