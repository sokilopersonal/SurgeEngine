using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class CameraPan : CameraPawn
    {
        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _cameraTransform.position = Vector3.Lerp(_lastPosition, _panData.position, Easings.Get(Easing.OutCubic, _factor));
            _cameraTransform.rotation = 
                Quaternion.LookRotation(Actor.transform.position -_cameraTransform.position, Vector3.up);
            
            _camera.fieldOfView = Mathf.Lerp(_lastFov, _panData.fov, Easings.Get(Easing.OutCubic, _factor));

            _factor += dt / _panData.easeTimeEnter;
            _factor = Mathf.Clamp01(_factor);
        }
    }
}