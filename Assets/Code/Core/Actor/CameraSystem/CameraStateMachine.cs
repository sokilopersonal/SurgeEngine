using System;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.CameraSystem
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
        
        public Vector3 LateOffset { get; private set; }

        public Vector3 direction;
        public Vector3 freeDirection;
        public Vector3 sideDirection;
        public Vector3 sideOffset;
        private float _directionTransitionFactor;
        
        public Camera camera;
        public Transform transform;

        public PanData currentData;

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

            LateOffset = Vector3.Lerp(LateOffset, Vector3.zero, 4f * dt);
            actorPosition = GetActorPosition();
            
            var kinematics = ActorContext.Context.kinematics;
            freeDirection = GetDirection(KinematicsMode.Free);
            if (kinematics.GetPath() != null) sideDirection = GetDirection(KinematicsMode.Side);
            direction = Vector3.Lerp(freeDirection, sideDirection, Easings.Get(Easing.Gens, _directionTransitionFactor));

            KinematicsMode mode = kinematics.mode;
            float factor = dt / 1.5f;
            if (mode is KinematicsMode.Free or KinematicsMode.Forward or KinematicsMode.Dash)
            {
                _directionTransitionFactor -= factor;
            }
            else if (mode is KinematicsMode.Side)
            {
                _directionTransitionFactor += factor;
            }
            
            _directionTransitionFactor = Mathf.Clamp01(_directionTransitionFactor);
            
            base.Tick(dt);
            
            transform.position = position;
            transform.rotation = rotation;
            camera.fieldOfView = fov;
        }

        private Vector3 GetActorPosition()
        {
            Vector3 basePosition = master.Actor.transform.position + LateOffset + Vector3.up * yOffset + Vector3.up * yLag;
            var kinematics = ActorContext.Context.kinematics;
            var path = kinematics.GetPath();
            KinematicsMode mode = kinematics.mode;

            if (mode is KinematicsMode.Free or KinematicsMode.Forward or KinematicsMode.Dash)
            {
                return basePosition;
            }
            else
            {
                SplineUtility.GetNearestPoint(path.Spline, 
                    path.transform.InverseTransformPoint(basePosition),
                    out _, 
                    out var f, 
                    12, 6);
                
                path.Evaluate(path.Spline, f, out var p, out var tg, out var up);
                
                SplineSample sample = new SplineSample
                {
                    pos = p,
                    tg = ((Vector3)tg).normalized,
                    up = up
                };
                
                float dot = Vector3.Dot(sample.tg, kinematics.transform.forward);
                Vector3 side = sample.tg;
                side *= 1.3f;
                
                if (dot < 0)
                {
                    side *= -1;
                }

                sideOffset = Vector3.Lerp(sideOffset, side, 2.75f * Time.deltaTime);
                
                return basePosition + sideOffset;
            }
        }
        
        public Vector3 GetDirection(KinematicsMode mode)
        {
            ActorBase actor = ActorContext.Context;
            Vector3 camDir;
            if (mode is KinematicsMode.Free or KinematicsMode.Forward or KinematicsMode.Dash)
            {
                var horizontal = Quaternion.AngleAxis(x, Vector3.up);
                var vertical = Quaternion.AngleAxis(y, Vector3.right);

                camDir = horizontal * vertical * Vector3.back;
            }
            else
            {
                var path = actor.kinematics.GetPath();
                SplineUtility.GetNearestPoint(path.Spline, 
                    path.transform.InverseTransformPoint(actor.transform.position),
                    out _, 
                    out var f, 
                    12, 6);

                path.Evaluate(path.Spline, f, out var p, out var tg, out var up);

                SplineSample sample = new SplineSample
                {
                    pos = p,
                    tg = ((Vector3)tg).normalized,
                    up = up
                };
                
                Vector3 plane = Vector3.Cross(sample.tg, -Vector3.up);
                camDir = plane * 4f;
                
                Debug.DrawRay(actor.transform.position, camDir, Color.red);
                
                Vector3 localPos = path.transform.TransformPoint(p);
                Debug.DrawRay(localPos, sample.tg, Color.green);
            }
            
            return camDir;
        }

        public void SetDirection(Vector3 forward, bool resetY = false)
        {
            Quaternion dir = Quaternion.LookRotation(forward, Vector3.up);
            x = dir.eulerAngles.y;
            y = !resetY ? dir.eulerAngles.x : 0f;
        }
        
        public void SetLateOffset(Vector3 offset)
        {
            LateOffset = offset;
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
                fov = fov,
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
                fov = fov,
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