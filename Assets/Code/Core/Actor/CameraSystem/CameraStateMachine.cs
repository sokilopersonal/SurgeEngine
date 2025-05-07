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

        public Vector3 actualDirection;
        public Vector3 defaultDirection;
        public Vector3 sideDirection;
        public Vector3 sideOffset;
        
        private Vector3 _sideOffsetVelocity;
        public float sideBlendFactor;
        public bool is2D;
        
        public Vector3 LateOffset { get; private set; }
        
        private float _directionTransitionFactor;
        
        public Camera camera;
        public Transform transform;

        public PanData currentData;
        
        public float blendFactor { get; private set; }
        public float interpolatedBlendFactor { get; private set; }

        private bool _isPointMarkerLoaded;

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
            
            defaultDirection = GetDirection();
            actualDirection = defaultDirection;

            OnStateEarlyAssign += _ =>
            {
                ResetBlendFactor();
            };
            
            master.Actor.kinematics.OnModeChange += mode => is2D = mode == KinematicsMode.Side;
        }

        public override void Tick(float dt)
        {
            if (currentData != null)
            {
                PanData baseData = currentData;
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
            actorPosition = GetActorPosition() + sideOffset;
            
            defaultDirection = GetDirection();
            if (master.Actor.kinematics.IsPathValid()) sideDirection = GetSideDirection();
            actualDirection = Vector3.Lerp(defaultDirection, sideDirection, Easings.Get(Easing.Gens, sideBlendFactor));
            
            if (is2D)
            {
                sideBlendFactor += dt;
            }
            else
            {
                sideBlendFactor -= dt;
            }
            
            sideBlendFactor = Mathf.Clamp01(sideBlendFactor);
            
            if (master.Actor.kinematics.IsPathValid()) sideOffset = 
                Vector3.SmoothDamp(sideOffset, GetSideOffset(), ref _sideOffsetVelocity, 0.2f);
            else
            {
                sideOffset = Vector3.SmoothDamp(sideOffset, Vector3.zero, ref _sideOffsetVelocity, 0.2f);
            }
            
            base.Tick(dt);
            
            transform.position = position;
            transform.rotation = rotation;
            camera.fieldOfView = fov;
        }

        private Vector3 GetActorPosition()
        {
            Vector3 basePosition = master.Actor.transform.position + LateOffset + Vector3.up * yOffset + Vector3.up * yLag;
            var kinematics = ActorContext.Context.kinematics;
            kinematics.GetPath();

            return basePosition;
        }
        
        public Vector3 GetDirection()
        {
            var horizontal = Quaternion.AngleAxis(x, Vector3.up);
            var vertical = Quaternion.AngleAxis(y, Vector3.right);
            
            return horizontal * vertical * Vector3.back;
        }
        
        private Vector3 GetSideDirection()
        {
            var path = master.Actor.kinematics.GetPath();
            var spline = path.Spline;
            SplineUtility.GetNearestPoint(spline, path.transform.InverseTransformPoint(master.Actor.transform.position),
                out _, out var f, 8, 4);

            path.Evaluate(spline, f, out var p, out var tg, out var up);

            var sample = new SplineSample
            {
                pos = p,
                tg = ((Vector3)tg).normalized,
                up = up
            };
            
            Vector3 plane = Vector3.Cross(sample.tg, -Vector3.up);
            return plane * 4f;
        }

        private Vector3 GetSideOffset()
        {
            var path = master.Actor.kinematics.GetPath();
            var spline = path.Spline;
            SplineUtility.GetNearestPoint(spline, path.transform.InverseTransformPoint(actorPosition),
                out _, out var f, 8, 4);
            
            path.Evaluate(spline, f, out var p, out var tg, out var up);

            var sample = new SplineSample
            {
                pos = p,
                tg = ((Vector3)tg).normalized,
                up = up
            };
            
            Vector3 side = sample.tg;
            float dot = Vector3.Dot(side, master.Actor.transform.forward);
            side *= 1.3f;

            if (dot < 0)
            {
                side *= -1;
            }
            
            return side;
        }

        public void SetDirection(Vector3 forward, bool resetY = false)
        {
            Quaternion dir = Quaternion.LookRotation(forward, Vector3.up).normalized;
            x = dir.eulerAngles.y;
            y = !resetY ? dir.eulerAngles.x : 0f;
            
            xAutoLook = 0f;
            yAutoLook = 0f;
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