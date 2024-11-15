using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class RestoreCameraPawn : NewModernState
    {
        private Vector3 _direction;
        private LastCameraData _lastData;
        
        public RestoreCameraPawn(Actor owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            _stateMachine.ResetBlendFactor();
            _stateMachine.RememberLastData();
            
            _lastData = _stateMachine.GetLastData();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _stateMachine.currentData = null;
        }

        public override void OnTick(float dt)
        {
            _lastData = _stateMachine.GetLastData();
            
            base.OnTick(dt);
            
            var cross = Vector3.Cross(_actor.transform.right, Vector3.up);
            SetDirection(cross);
            
            _stateMachine.camera.fieldOfView = Mathf.Lerp(_lastData.fov, 60f, _stateMachine.interpolatedBlendFactor);
            if (_stateMachine.blendFactor >= 1f)
            {
                _stateMachine.SetState<NewModernState>();
            }
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            _stateMachine.position = Vector3.Lerp(_lastData.position, targetPosition, _stateMachine.interpolatedBlendFactor);
        }

        protected override void SetRotation(Vector3 actorPosition)
        {
            _stateMachine.rotation = Quaternion.Slerp(_lastData.rotation, Quaternion.LookRotation(actorPosition - _stateMachine.position), _stateMachine.interpolatedBlendFactor);
        }

        protected override void AutoLookDirection()
        {
        }
    }
}