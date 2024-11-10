using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem
{
    public class MasterCamera : FStateMachine
    {
        public float x;
        public float y;

        public Vector3 position;
        public Quaternion rotation;
        
        public Camera camera;
        public Transform transform;

        public MasterCamera(Camera camera, Transform transform)
        {
            this.camera = camera;
            this.transform = transform;
        }

        public override void Tick(float dt)
        {
            base.Tick(dt);
            
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}