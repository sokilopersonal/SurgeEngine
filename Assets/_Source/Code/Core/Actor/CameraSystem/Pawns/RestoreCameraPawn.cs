using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns
{
    public class RestoreCameraPawn : NewModernState
    {
        public RestoreCameraPawn(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            _lastData = _stateMachine.RememberRelativeLastData();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.Distance = Mathf.Lerp(_lastData.distance, _stateMachine.StartDistance, _stateMachine.interpolatedBlendFactor);
            _stateMachine.VerticalOffset = Mathf.Lerp(_lastData.yOffset, _stateMachine.StartVerticalOffset, _stateMachine.interpolatedBlendFactor);
            
            _stateMachine.FOV = Mathf.Lerp(_lastData.fov, _stateMachine.FOV, _stateMachine.interpolatedBlendFactor);
            if (_stateMachine.blendFactor >= 1f)
            {
                _stateMachine.SetState<NewModernState>();
            }
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            Vector3 center = _stateMachine.ActorPosition;
            Vector3 diff = targetPosition - center;
            _stateMachine.Position = Vector3.Slerp(_lastData.position, diff, _stateMachine.interpolatedBlendFactor);
            _stateMachine.Position += center;
        }

        protected override void SetRotation(Vector3 actorPosition)
        {
            _stateMachine.Rotation = Quaternion.Lerp(_lastData.rotation, 
                Quaternion.LookRotation(actorPosition + _stateMachine.Transform.TransformDirection(new Vector3(_stateMachine.PanLookOffset.x, 0, 0)) - _stateMachine.Position), _stateMachine.interpolatedBlendFactor);
        }

        protected override void AutoLook(float multiplier)
        {
            base.AutoLook(multiplier * _stateMachine.blendFactor);
        }
    }
}