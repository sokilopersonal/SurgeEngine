using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class VerticalCameraPan : DefaultModernPawn
    {
        private new VerticalPanData _panData;
        
        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            
        }

        protected override void GetLook()
        {
        }

        protected override void AutoFollow()
        {
            
        }
        
        public override void SetPosition(Vector3 pos)
        {
            _cameraTransform.position = Vector3.Lerp(_lastPosition, pos, Easings.Get(Easing.OutCubic, _factor));
        }

        public override void SetRotation(Vector3 pos)
        {
            Vector3 endPos = _panData.position;
            endPos.y += _panData.groundOffset;
            Vector3 dir = endPos - _cameraTransform.position;
            Debug.DrawLine(_cameraTransform.position, dir, Color.red);
            Quaternion rot = Quaternion.LookRotation(dir);
            _cameraTransform.rotation = Quaternion.Slerp(_cameraTransform.rotation, rot, 7f * Time.deltaTime);
        }

        public override void SetData(PanData data)
        {
            base.SetData(data);
            
            _panData = (VerticalPanData)data;
        }
    }
}