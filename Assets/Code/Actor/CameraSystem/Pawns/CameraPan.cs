using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class CameraPan : CameraPawn
    {
        private PanData _panData;
        private bool _calculatedDiff;
        
        public CameraPan(Actor owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _calculatedDiff = false;
            
            _stateMachine.ResetBlendFactor();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _panData = (PanData)_data;

            if (!_calculatedDiff)
            {
                _stateMachine.RememberRelativeLastData();
                _calculatedDiff = true;
            }
            var last = _stateMachine.GetLastData();
            
            Vector3 center = _actor.transform.position;
            Vector3 diff = _panData.position - center;
            _stateMachine.position = Vector3.Lerp(last.position, diff, _stateMachine.interpolatedBlendFactor);
            _stateMachine.position += center;
            
            var rotation = Quaternion.LookRotation(_actor.transform.position - _stateMachine.position, Vector3.up);
            _stateMachine.rotation = Quaternion.Slerp(last.rotation, rotation, _stateMachine.interpolatedBlendFactor);
            _stateMachine.camera.fieldOfView = Mathf.Lerp(last.fov, _panData.fov, _stateMachine.interpolatedBlendFactor);
        }
    }
}