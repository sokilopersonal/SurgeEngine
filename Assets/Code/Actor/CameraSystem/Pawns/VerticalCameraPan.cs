using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class VerticalCameraPan : DefaultModernPawn
    {
        private new VerticalPanData _panData;
        
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        public override void OnEnter()
        {
            base.OnEnter();
            
            _lastPosition = _cameraTransform.position;
            _lastRotation = _cameraTransform.rotation;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            //_cameraTransform.position = Vector3.Lerp(_lastPosition, pos, Easings.Get(Easing.OutCubic, _factor));
            
            Vector3 endPos = _panData.position;
            endPos.y += _panData.groundOffset;
            Vector3 dir = endPos - _cameraTransform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            _cameraTransform.rotation = Quaternion.Slerp(_lastRotation, rot, Easings.Get(Easing.OutCubic, _factor));
            
            _factor += dt / _panData.easeTimeEnter;
            _factor = Mathf.Clamp01(_factor);
        }

        public override void SetPosition(Vector3 pos)
        {
        }

        public override void SetRotation(Vector3 pos)
        {
            
        }

        public override void SetData(PanData data)
        {
            base.SetData(data);
            
            _panData = (VerticalPanData)data;
        }
    }
}