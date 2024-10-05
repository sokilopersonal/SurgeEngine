using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class RestoreCameraPawn : DefaultModernPawn
    {
        private PanData _panData;

        private float _factor;

        public override void OnEnter()
        {
            base.OnEnter();

            _tempY = 0;
            SetRotationAxis(actor.transform.forward);
            
            _factor = 0;
        }

        public override void OnTick(float dt)
        {
            _factor += dt / _panData.easeOutTime;
            _factor = Mathf.Clamp01(_factor);
            
            base.OnTick(dt);
            
            if (_factor >= 1)
            {
                var defaultModern = actor.camera.stateMachine.GetState<DefaultModernPawn>();
                defaultModern.SetRotation(_x, _y);
                defaultModern.SetTempZ(_tempZ);
                defaultModern.SetTempY(_tempY);
                actor.camera.stateMachine.SetState<DefaultModernPawn>();
            }
        }

        protected override void GetLook()
        {
        }

        public override void SetPosition(Vector3 pos)
        {
            _cameraTransform.position = Vector3.Lerp(_panData.position, pos, Easings.Get(Easing.OutCubic, _factor));
        }

        protected override void SetFieldOfView(float fov)
        {
            _camera.fieldOfView = Mathf.Lerp(_panData.fov, fov, Easings.Get(Easing.OutCubic, _factor));
        }

        public void SetData(PanData data)
        {
            _panData = data;

            Debug.Log(_panData);
        }
    }
}