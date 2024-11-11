using SurgeEngine.Code.CameraSystem.Pawns;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem
{
    public class MasterCamera : FStateMachine
    {
        public float x;
        public float y;
        
        public float xAutoLook;
        public float yAutoLook;
        
        public float yLag;
        public float yLagVelocity;
        public float zLag;
        public float zLagVelocity;

        public Vector3 position;
        public Quaternion rotation;
        
        public Camera camera;
        public Transform transform;

        public PanData currentData;

        public MasterCamera(Camera camera, Transform transform)
        {
            this.camera = camera;
            this.transform = transform;
        }

        public override void LateTick(float dt)
        {
            base.LateTick(dt);
            
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}