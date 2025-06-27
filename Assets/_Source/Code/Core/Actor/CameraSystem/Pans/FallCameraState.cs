using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class FallCameraState : CameraBasePan<PanData>
    {
        public FallCameraState(ActorBase owner) : base(owner)
        {
            _panData = new PanData
            {
                easeTimeEnter = 0.75f,
            };
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _stateMachine.CurrentData = _panData;
            _lastData = _stateMachine.RememberLastData();
        }

        public override void OnExit()
        {
            base.OnExit();

            _stateMachine.CurrentData = null;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.SetRotationInterpolated(_actor.transform.position, _lastData.rotation);
            _stateMachine.FOV = Mathf.Lerp(_lastData.fov, 50, _stateMachine.interpolatedBlendFactor);
        }
    }
}