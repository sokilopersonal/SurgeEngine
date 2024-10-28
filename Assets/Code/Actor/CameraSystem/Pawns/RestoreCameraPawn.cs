using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class RestoreCameraPawn : DefaultModernPawn
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            Input.CameraLock(true);

            _tempY = 0;
            SetRotationAxis(Actor.transform.forward);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Input.CameraLock(false);
        }

        public override void OnTick(float dt)
        {
            _factor += dt / _panData.easeTimeExit;
            _factor = Mathf.Clamp01(_factor);
            
            base.OnTick(dt);
            
            if (_factor >= 1)
            {
                var defaultModern = Actor.camera.stateMachine.GetState<DefaultModernPawn>();
                defaultModern.SetRotationValues(_x, _y);
                defaultModern.SetTempZ(_tempZ);
                defaultModern.SetTempY(_tempY);
                Actor.camera.stateMachine.SetState<DefaultModernPawn>();
            }
        }

        public override void SetPosition(Vector3 pos)
        {
            _cameraTransform.position = Vector3.Lerp(_panData.position != Vector3.zero ? _panData.position : _lastPosition, pos, Easings.Get(Easing.OutCubic, _factor));
        }

        protected override void SetFieldOfView(float fov)
        {
            _camera.fieldOfView = Mathf.Lerp(_panData.fov, fov, Easings.Get(Easing.OutCubic, _factor));
        }

        public override void SetData(PanData data)
        {
            _panData = data;
        }
    }
}