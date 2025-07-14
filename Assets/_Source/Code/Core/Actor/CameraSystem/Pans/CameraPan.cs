using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class CameraPan : CameraBasePan<PanData>
    {
        public CameraPan(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _lastData = _stateMachine.RememberLastData();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _stateMachine.SetDirection(_stateMachine.Transform.transform.forward);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            StatePosition = _panData.position;
            StateRotation = Quaternion.LookRotation(_actor.transform.position - _panData.position);
            StateFOV = _panData.fov;
            //_stateMachine.FOV = Mathf.Lerp(_lastData.fov, _panData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}