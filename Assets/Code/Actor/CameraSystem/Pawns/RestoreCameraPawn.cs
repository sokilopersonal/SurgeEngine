using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class RestoreCameraPawn : NewModernState
    {
        private float _factor;
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private float _lastFOV;
        
        public RestoreCameraPawn(Actor owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _lastPosition = _stateMachine.position;
            _lastRotation = _stateMachine.rotation;
            _lastFOV = _stateMachine.camera.fieldOfView;
            
            _factor = 0f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.camera.fieldOfView = Mathf.Lerp(_lastFOV, 60f, Easings.Get(Easing.OutCubic, _factor));
            
            _factor += dt / _stateMachine.currentData.easeTimeExit;
            if (_factor > 1f)
            {
                _stateMachine.SetState<NewModernState>();
            }
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            _stateMachine.position = Vector3.Lerp(_lastPosition, targetPosition, Easings.Get(Easing.OutCubic, _factor));
        }

        protected override void SetRotation(Vector3 actorPosition)
        {
            _stateMachine.rotation = Quaternion.LookRotation(actorPosition - _stateMachine.position);
        }
    }
}