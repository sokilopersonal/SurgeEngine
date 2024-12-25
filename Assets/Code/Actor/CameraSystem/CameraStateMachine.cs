using System;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem
{
    [Serializable]
    public class CameraStateMachine : FStateMachine
    {
        public ActorCamera master;
        
        public float x;
        public float y;
        
        public float boostDistance;
        public float boostFov;
        
        public float xAutoLook;
        public float yAutoLook;
        
        public float yLag;
        public float yLagVelocity;
        public float zLag;
        public float zLagVelocity;

        public float baseFov = 55f;
        public float fov;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 lookOffset;
        
        public Vector3 lastPosition;
        public Quaternion lastRotation;
        public float lastFOV;
        
        public Camera camera;
        public Transform transform;

        public object currentData;

        public float blendFactor { get; private set; }
        public float interpolatedBlendFactor { get; private set; }

        public CameraStateMachine(Camera camera, Transform transform, ActorCamera master)
        {
            this.camera = camera;
            this.transform = transform;
            this.master = master;

            boostDistance = 1f;
            boostFov = 1f;
            fov = baseFov;
        }

        public override void Tick(float dt)
        {
            if (currentData != null)
            {
                PanData baseData = (PanData)currentData;
                float easeTime = !IsExact<RestoreCameraPawn>() ? baseData.easeTimeEnter : baseData.easeTimeExit;
                blendFactor += dt / easeTime;
                blendFactor = Mathf.Clamp01(blendFactor);
                interpolatedBlendFactor = Easings.Get(Easing.Gens, blendFactor);

                if (baseData.allowRotation)
                {
                    Vector2 v = ActorContext.Context.input.lookVector;
                    v = Vector3.ClampMagnitude(v, 2f);
                    lookOffset = Vector3.Lerp(lookOffset, v, SurgeMath.Smooth(1 - 0.75f));
                }
            }
            else
            {
                lookOffset = Vector3.zero;
            }
            
            base.Tick(dt);
            
            transform.position = position;
            transform.rotation = rotation;
            camera.fieldOfView = fov;
        }

        public void SetDirection(Vector3 forward, bool resetY = false)
        {
            Quaternion direction = Quaternion.LookRotation(forward, Vector3.up);
            x = direction.eulerAngles.y;
            y = !resetY ? direction.eulerAngles.x : 0f;
        }
        
        public void ResetBlendFactor()
        {
            blendFactor = 0f;
            interpolatedBlendFactor = 0f;
        }

        public LastCameraData RememberLastData()
        {
            lastPosition = position;
            lastRotation = rotation;
            lastFOV = camera.fieldOfView;

            return new LastCameraData
            {
                position = lastPosition,
                rotation = lastRotation,
                fov = lastFOV
            };
        }
        
        public LastCameraData RememberRelativeLastData()
        {
            Vector3 center = master.transform.position; // Player
            lastPosition = position - center;
            lastRotation = rotation;
            lastFOV = camera.fieldOfView;
            
            return new LastCameraData
            {
                position = lastPosition,
                rotation = lastRotation,
                fov = lastFOV
            };
        }
        
        public LastCameraData GetLastData()
        {
            return new LastCameraData
            {
                position = lastPosition,
                rotation = lastRotation,
                fov = lastFOV
            };
        }
    }

    public class LastCameraData
    {
        public Vector3 position;
        public Quaternion rotation;
        public float fov;
    }
}