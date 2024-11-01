using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public abstract class CameraPawn : FActorState
    {
        protected Camera _camera;
        protected Transform _cameraTransform;

        protected PanData _panData;
        protected float _factor;
        protected Vector3 _lastPosition;
        protected float _lastFov;

        protected CameraPawn(Actor owner) : base(owner)
        {
            _camera = owner.camera.GetCamera();
            _cameraTransform = _camera.transform;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _lastPosition = _cameraTransform.position;
            _lastFov = _camera.fieldOfView;

            _factor = 0f;
        }
        
        public virtual void SetData(PanData data)
        {
            _panData = data;
        }
    }
}