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

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stateMachine.ResetBlendFactor();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _panData = (PanData)_data;
            
            Transform cam = _stateMachine.transform;
            _stateMachine.position = Vector3.Lerp(_lastPosition, _panData.position, _stateMachine.interpolatedBlendFactor);
            var rotation = Quaternion.LookRotation(_actor.transform.position - cam.position, Vector3.up);
            _stateMachine.rotation = Quaternion.Slerp(_lastRotation, rotation, _stateMachine.interpolatedBlendFactor);
            _stateMachine.camera.fieldOfView = Mathf.Lerp(_lastFOV, _panData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}