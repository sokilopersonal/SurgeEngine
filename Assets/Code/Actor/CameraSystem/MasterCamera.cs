using SurgeEngine.Code.CameraSystem.Pawns;
using SurgeEngine.Code.Custom;
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

        public object currentData;

        public float blendFactor { get; private set; }
        public float interpolatedBlendFactor { get; private set; }

        public MasterCamera(Camera camera, Transform transform)
        {
            this.camera = camera;
            this.transform = transform;
        }

        public override void Tick(float dt)
        {
            if (currentData != null)
            {
                PanData baseData = (PanData)currentData;
                float easeTime = !Is<RestoreCameraPawn>() ? baseData.easeTimeEnter : baseData.easeTimeExit;
                blendFactor += dt / easeTime;
                blendFactor = Mathf.Clamp01(blendFactor);
                interpolatedBlendFactor = Easings.Get(Easing.Gens, blendFactor);
            }
            
            base.Tick(dt);
        }

        public override void LateTick(float dt)
        {
            base.LateTick(dt);
            
            transform.position = position;
            transform.rotation = rotation;
        }
        
        public void ResetBlendFactor()
        {
            blendFactor = 0f;
            interpolatedBlendFactor = 0f;
        }
    }
}