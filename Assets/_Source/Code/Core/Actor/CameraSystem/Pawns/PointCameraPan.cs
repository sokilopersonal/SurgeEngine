using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns
{
    public class PointCameraPan : NewModernState
    {
        private PointPanData _pData => _panData as PointPanData;
        
        public PointCameraPan(ActorBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _lastData = _stateMachine.RememberRelativeLastData();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _stateMachine.SetDirection(_stateMachine.Transform.transform.forward);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.SetDirection(_pData.Forward);
            
            _stateMachine.Distance = Mathf.Lerp(_lastData.distance, _pData.distance, _stateMachine.interpolatedBlendFactor);
            _stateMachine.VerticalOffset = Mathf.Lerp(_lastData.yOffset, _pData.yOffset, _stateMachine.interpolatedBlendFactor);
            
            _stateMachine.FOV = Mathf.Lerp(_lastData.fov, _pData.fov, _stateMachine.interpolatedBlendFactor);
        }
        
        protected override void SetPosition(Vector3 targetPosition)
        {
            Vector3 center = _stateMachine.ActorPosition;
            Vector3 diff = targetPosition + _stateMachine.Transform.TransformDirection(_pData.offset) - center;
            _stateMachine.Position = Vector3.Slerp(_lastData.position, diff, _stateMachine.interpolatedBlendFactor);
            _stateMachine.Position += center;
        }
        
        protected override void SetRotation(Vector3 actorPosition)
        {
            Vector3 look = 
                actorPosition
                + _stateMachine.Transform.TransformDirection(_stateMachine.LookOffset)
                + _stateMachine.Transform.TransformDirection(_pData.localLookOffset)
                + _stateMachine.Transform.TransformDirection(_pData.offset)
                - _stateMachine.Position;
            
            if (look != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(look);
            
                _stateMachine.Rotation = Quaternion.Lerp(_lastData.rotation, rot, _stateMachine.interpolatedBlendFactor);
            }
        }

        protected override void LookAxis()
        {
            
        }
    }
}