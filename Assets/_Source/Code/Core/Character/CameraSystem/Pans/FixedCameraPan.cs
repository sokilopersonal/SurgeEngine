using SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine._Source.Code.Core.Character.System;

namespace SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans
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