using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class CameraPan : CameraBasePan<PanData>
    {
        public CameraPan(CharacterBase owner) : base(owner)
        {
            
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
            StateRotation = Quaternion.LookRotation(Character.transform.position - _panData.position);
            StateFOV = _panData.fov;
        }
    }
}