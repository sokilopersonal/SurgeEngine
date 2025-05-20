using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns
{
    public class FallCameraState : CameraState<PanData>
    {
        public FallCameraState(ActorBase owner) : base(owner)
        {
            _panData = new PanData()
            {
                easeTimeEnter = 0.75f,
            };
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _stateMachine.currentData = _panData;
            _lastData = _stateMachine.RememberLastData();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Quaternion rotation = Quaternion.LookRotation(_actor.transform.position - _stateMachine.position);
            _stateMachine.rotation = Quaternion.Lerp(_lastData.rotation, rotation, _stateMachine.interpolatedBlendFactor);
            _stateMachine.fov = Mathf.Lerp(_lastData.fov, 50, _stateMachine.interpolatedBlendFactor);
        }
    }
}