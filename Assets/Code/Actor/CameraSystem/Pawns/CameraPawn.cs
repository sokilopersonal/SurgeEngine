using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public abstract class CameraPawn : CState
    {
        protected Vector3 _lastPosition;
        protected Quaternion _lastRotation;
        protected float _lastFOV;

        protected CameraPawn(Actor owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _lastPosition = _stateMachine.position;
            _lastRotation = _stateMachine.rotation;
            _lastFOV = _stateMachine.camera.fieldOfView;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.currentData = _data as PanData;
        }
    }
}