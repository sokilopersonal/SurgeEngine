using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class FixedCameraPan : CameraBasePan<FixPanData>
    {
        public FixedCameraPan(CharacterBase owner) : base(owner)
        {
            
        }
        
        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            StatePosition = _panData.position;
            StateRotation = _panData.target;
            StateFOV = _panData.fov;
        }
    }
}