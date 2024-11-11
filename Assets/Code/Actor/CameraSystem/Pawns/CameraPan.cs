using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class CameraPan : CameraPawn
    {
        private PanData _panData;
        
        public CameraPan(Actor owner) : base(owner)
        {
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _panData = (PanData)_data;

            Transform cam = _stateMachine.transform;
            _stateMachine.position = Vector3.Lerp(_lastPosition, _panData.position, Easings.Get(Easing.OutCubic, _factor));
            _stateMachine.rotation = Quaternion.LookRotation(_actor.transform.position - cam.position, Vector3.up);
            _stateMachine.camera.fieldOfView = Mathf.Lerp(_lastFOV, _panData.fov, Easings.Get(Easing.OutCubic, _factor));
            
            _factor += dt / _panData.easeTimeEnter;
        }
    }
}