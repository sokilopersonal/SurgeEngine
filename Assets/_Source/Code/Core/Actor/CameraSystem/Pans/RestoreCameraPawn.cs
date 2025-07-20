using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class RestoreCameraPawn : NewModernState
    {
        private LastCameraData _lastData;
        
        public RestoreCameraPawn(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.FOV = Mathf.Lerp(_lastData.fov, _stateMachine.FOV, _stateMachine.interpolatedBlendFactor);
            if (_stateMachine.blendFactor >= 1f)
            {
                _stateMachine.SetState<NewModernState>();
            }
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            StatePosition = targetPosition;
        }

        protected override void SetRotation(Vector3 actorPosition)
        {
            StateRotation = Quaternion.LookRotation(actorPosition - StatePosition);
        }

        protected override void AutoLook(float multiplier)
        {
            base.AutoLook(multiplier * _stateMachine.blendFactor);
        }
    }
}