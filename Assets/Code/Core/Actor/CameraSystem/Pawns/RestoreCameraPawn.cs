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
            _stateMachine.ResetBlendFactor();
            _lastData = _stateMachine.RememberRelativeLastData();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.distance = Mathf.Lerp(_lastData.distance, _stateMachine.startDistance, _stateMachine.interpolatedBlendFactor);
            _stateMachine.yOffset = Mathf.Lerp(_lastData.yOffset, _stateMachine.startYOffset, _stateMachine.interpolatedBlendFactor);
            
            _stateMachine.fov = Mathf.Lerp(_lastData.fov, _stateMachine.fov, _stateMachine.interpolatedBlendFactor);
            if (_stateMachine.blendFactor >= 1f)
            {
                _stateMachine.SetState<NewModernState>();
            }
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            Vector3 center = _stateMachine.actorPosition;
            Vector3 diff = targetPosition - center;
            _stateMachine.position = Vector3.Slerp(_lastData.position, diff, _stateMachine.interpolatedBlendFactor);
            _stateMachine.position += center;
        }

        protected override void SetRotation(Vector3 actorPosition)
        {
            _stateMachine.rotation = Quaternion.Lerp(_lastData.rotation, 
                Quaternion.LookRotation(actorPosition + _stateMachine.transform.TransformDirection(new Vector3(_master.lookOffset.x, 0, 0)) - _stateMachine.position), _stateMachine.interpolatedBlendFactor);
        }

        protected override void AutoLook(float multiplier)
        {
            base.AutoLook(multiplier * _stateMachine.blendFactor);
        }
    }
}