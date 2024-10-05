using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public abstract class CameraPawn : FActorState
    {
        protected Camera _camera;
        protected Transform _cameraTransform;

        private void Awake()
        {
            _camera = Camera.main;
            _cameraTransform = _camera.transform;
        }
    }
}