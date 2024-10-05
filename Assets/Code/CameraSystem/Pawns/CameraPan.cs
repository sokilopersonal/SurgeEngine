using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class CameraPan : CameraPawn
    {
        private PanData _panData;

        private float _factor;
        private Vector3 _lastPosition;
        private float _lastFov;

        public override void OnEnter()
        {
            base.OnEnter();

            _factor = 0;
            _lastPosition = _cameraTransform.position;
            _lastFov = _camera.fieldOfView;
        }
        
        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _cameraTransform.position = Vector3.Lerp(_lastPosition, _panData.position, Easings.Get(Easing.OutCubic, _factor));
            _cameraTransform.rotation = 
                Quaternion.LookRotation(actor.transform.position -_cameraTransform.position, Vector3.up);
            
            _camera.fieldOfView = Mathf.Lerp(_lastFov, _panData.fov, Easings.Get(Easing.OutCubic, _factor));

            _factor += dt / _panData.easeInTime;
            _factor = Mathf.Clamp01(_factor);
        }
        
        public void SetData(PanData data)
        {
            _panData = data;
        }
    }
}