using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem
{
    public class CameraStateMachine : FStateMachine
    {
        public ActorCamera Master { get; }
        public Camera Camera { get; }
        public Transform Transform { get; }

        public float Yaw { get; set; }
        public float Pitch { get; set; }

        public float StartDistance { get; }
        public float StartVerticalOffset { get; }
        public float Distance { get; set; }
        public float VerticalOffset { get; set; }

        public float YawAuto { get; set; }
        public float PitchAuto { get; set; }

        public float VerticalLag { get; set; }
        public float VerticalLagVelocity;
        public float ForwardLag { get; set; }
        public float ForwardLagVelocity;

        public float BaseFov => 55f;
        public float FOV { get; set; }

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 LookOffset;
        public Vector3 PanLookOffset;

        public Vector3 ActorPosition { get; private set; }
        public Vector3 ActualDirection { get; private set; }
        private Vector3 DefaultDirection { get; set; }
        private Vector3 SideDirection { get; set; }
        private Vector3 SideOffset { get; set; }
        public float SideBlendFactor { get; private set; }
        private Vector3 _sideOffsetVelocity;
        public bool Is2D { get; private set; }
        
        public PanData CurrentData { get; set; }
        public float blendFactor { get; private set; }
        public float interpolatedBlendFactor { get; private set; }

        private Vector3 LateOffset { get; set; }
        
        private readonly ActorBase _actor;
        
        public CameraStateMachine(Camera camera, Transform transform, ActorBase actor, ActorCamera master)
        {
            Camera = camera;
            Transform = transform;
            _actor = actor;
            Master = actor.Camera;

            StartDistance = master.Distance;
            StartVerticalOffset = master.YOffset;
            Distance = StartDistance;
            VerticalOffset = StartVerticalOffset;
            
            FOV = BaseFov;
            
            DefaultDirection = GetCameraDirection();
            ActualDirection = DefaultDirection;

            OnStateEarlyAssign += _ =>
            {
                ResetBlendFactor();
            };
            
            actor.Kinematics.OnModeChange += mode =>
            {
                if (Is2D && mode != KinematicsMode.Side)
                {
                    SetDirection(master.Actor.transform.forward);
                }
                
                Is2D = mode == KinematicsMode.Side;
            };
        }

        public override void Tick(float dt)
        {
            PanBlend(dt);
            Setup(dt);

            base.Tick(dt);
            
            Transform.position = Position;
            Transform.rotation = Rotation;
            Camera.fieldOfView = FOV;
        }

        private void Setup(float dt)
        {
            LateOffset = Vector3.Lerp(LateOffset, Vector3.zero, 4f * dt);
            
            ActorPosition = GetActorWorldPosition() + SideOffset;
            DefaultDirection = GetCameraDirection();
            if (Master.Actor.Kinematics.IsPathValid())
            {
                SideDirection = GetSideCameraDirection();
            }
            ActualDirection = Vector3.Lerp(DefaultDirection, SideDirection, Easings.Get(Easing.Gens, SideBlendFactor));
            
            if (Is2D)
            {
                SideBlendFactor += dt;
            }
            else
            {
                SideBlendFactor -= dt;
            }
            
            SideBlendFactor = Mathf.Clamp01(SideBlendFactor);

            if (!_actor.Kinematics.IsPathValid() || !Is2D)
            {
                SideOffset = Vector3.SmoothDamp(SideOffset, Vector3.zero, ref _sideOffsetVelocity, 0.2f);
            }
            else
            {
                SideOffset = Vector3.SmoothDamp(SideOffset, GetSideOffset(), ref _sideOffsetVelocity, 0.2f);
            }
        }

        private void PanBlend(float dt)
        {
            bool isExit = IsExact<RestoreCameraPawn>();
            if (CurrentData != null)
            {
                PanData baseData = CurrentData;
                float easeTime = !isExit ? baseData.easeTimeEnter : baseData.easeTimeExit;
                blendFactor += dt / easeTime;
                blendFactor = Mathf.Clamp01(blendFactor);
                interpolatedBlendFactor = Easings.Get(Easing.Gens, blendFactor);

                if (baseData.allowRotation)
                {
                    Vector2 v = _actor.Input.lookVector;
                    v = Vector3.ClampMagnitude(v, 2f);
                    PanLookOffset = Vector3.Lerp(PanLookOffset, v, 6f * dt);
                }
            }
            else
            {
                PanLookOffset = Vector3.Lerp(PanLookOffset, Vector3.zero, 12f * dt);;
            }

            if (isExit)
            {
                if (blendFactor >= 1f)
                {
                    CurrentData = null;
                }
            }
        }

        private Vector3 GetActorWorldPosition()
        {
            Vector3 basePosition = _actor.transform.position + LateOffset + Vector3.up * VerticalOffset + Vector3.up * VerticalLag;

            return basePosition;
        }
        
        public Vector3 GetCameraDirection()
        {
            var horizontal = Quaternion.AngleAxis(Yaw, Vector3.up);
            var vertical = Quaternion.AngleAxis(Pitch, Vector3.right);
            
            return horizontal * vertical * Vector3.back;
        }
        
        private Vector3 GetSideCameraDirection()
        {
            var path = _actor.Kinematics.GetPath();
            path.EvaluateWorld(out var pos, out var tg, out var up, out var right);
            
            return right * 3.5f;
        }

        private Vector3 GetSideOffset()
        {
            var path = _actor.Kinematics.GetPath();
            
            path.EvaluateWorld(out var pos, out var tg, out var up, out var right);
            
            Vector3 side = tg;
            float dot = Vector3.Dot(side, _actor.transform.forward);
            side *= 0.5f;

            if (dot < 0)
            {
                side *= -1;
            }
            
            return side;
        }

        private void ResetBlendFactor()
        {
            blendFactor = 0f;
            interpolatedBlendFactor = 0f;
        }

        public void SetDirection(Vector3 forward, bool resetY = false)
        {
            Quaternion dir = Quaternion.LookRotation(forward, Vector3.up).normalized;
            Yaw = dir.eulerAngles.y;
            Pitch = !resetY ? dir.eulerAngles.x : 0f;
            
            YawAuto = 0f;
            PitchAuto = 0f;
        }

        public void SetLateOffset(Vector3 offset)
        {
            LateOffset = offset;
        }

        public LastCameraData RememberLastData()
        {
            return new LastCameraData
            {
                position = Position,
                rotation = Rotation,
                fov = FOV,
                distance = Distance,
                yOffset = VerticalOffset
            };
        }
        
        public LastCameraData RememberRelativeLastData()
        {
            Vector3 center = ActorPosition; // Player
            return new LastCameraData
            {
                position = Position - center,
                rotation = Rotation,
                fov = FOV,
                distance = Distance,
                yOffset = VerticalOffset
            };
        }
        
        public LastCameraData GetLastData()
        {
            return new LastCameraData
            {
                position = Position,
                rotation = Rotation,
                fov = FOV
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