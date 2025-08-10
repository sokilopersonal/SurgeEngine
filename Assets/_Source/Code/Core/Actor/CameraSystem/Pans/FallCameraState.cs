using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class FallCameraState : CameraBasePan<PanData>
    {
        public FallCameraState(CharacterBase owner) : base(owner)
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
            
            StatePosition = _stateMachine.Transform.position;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            StateRotation = Quaternion.LookRotation(Character.transform.position - StatePosition);
            StateFOV = _panData.fov;
        }
    }
}