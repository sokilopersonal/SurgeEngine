using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class NormalCameraPan : NewModernState, IPanState<NormalPanData>
    {
        private LastCameraData _lastData;
        private NormalPanData _nData;
        
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
            
            _stateMachine.Distance = Mathf.Lerp(_lastData.distance, _nData.distance, _stateMachine.interpolatedBlendFactor);
            _stateMachine.VerticalOffset = Mathf.Lerp(_lastData.yOffset, _nData.yOffset, _stateMachine.interpolatedBlendFactor);
            
            _stateMachine.FOV = Mathf.Lerp(_lastData.fov, _nData.fov, _stateMachine.interpolatedBlendFactor);
        }

        public void SetData(NormalPanData data)
        {
            _nData = data;
            _stateMachine.CurrentData = data;
        }
    }
}