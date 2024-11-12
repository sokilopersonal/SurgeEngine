using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class RestoreCameraPawn : NewModernState
    {
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private float _lastFOV;
        
        public RestoreCameraPawn(Actor owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stateMachine.ResetBlendFactor();
            
            _lastPosition = _stateMachine.position;
            _lastRotation = _stateMachine.rotation;
            _lastFOV = _stateMachine.camera.fieldOfView;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.camera.fieldOfView = Mathf.Lerp(_lastFOV, 60f, _stateMachine.interpolatedBlendFactor);
            if (_stateMachine.blendFactor >= 1f)
            {
                _stateMachine.SetState<NewModernState>();
            }
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            _stateMachine.position = Vector3.Lerp(_lastPosition, targetPosition, _stateMachine.interpolatedBlendFactor);
        }

        protected override void SetRotation(Vector3 actorPosition)
        {
            _stateMachine.rotation = Quaternion.Lerp(_lastRotation, Quaternion.LookRotation(actorPosition - _stateMachine.position), _stateMachine.interpolatedBlendFactor);
        }
    }
}