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

        public float startDistance;
        public float startYOffset;
        public float distance;
        public float yOffset;
        
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
        public Vector3 actorPosition;
        
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

            startDistance = master.distance;
            startYOffset = master.yOffset;
            distance = startDistance;
            yOffset = startYOffset;

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

            actorPosition = GetActorPosition();
            
            base.Tick(dt);
            
            transform.position = position;
            transform.rotation = rotation;
            camera.fieldOfView = fov;
        }

        private Vector3 GetActorPosition()
        {
            return master.transform.position + Vector3.up * yOffset + Vector3.up * yLag;
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
            return new LastCameraData
            {
                position = position,
                rotation = rotation,
                fov = camera.fieldOfView,
                distance = distance,
                yOffset = yOffset
            };
        }
        
        public LastCameraData RememberRelativeLastData()
        {
            Vector3 center = actorPosition; // Player
            return new LastCameraData
            {
                position = position - center,
                rotation = rotation,
                fov = camera.fieldOfView,
                distance = distance,
                yOffset = yOffset
            };
        }
        
        public LastCameraData GetLastData()
        {
            return new LastCameraData
            {
                position = position,
                rotation = rotation,
                fov = fov
            };
        }
    }

    public class LastCameraData
    {
        public Vector3 position;
        public Quaternion rotation;
        public float fov;
        public float distance;
        public float yOffset;
    }
}