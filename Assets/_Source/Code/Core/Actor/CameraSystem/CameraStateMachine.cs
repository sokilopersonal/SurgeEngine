using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects;
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

        public float YawAuto { get; set; }
        public float PitchAuto { get; set; }

        public float BaseFov => 50f;
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
        private List<ChangeCameraVolume> _volumes;
        public IReadOnlyList<ChangeCameraVolume> Volumes => _volumes;
        private ChangeCameraVolume _lastTop;
        public int VolumeCount => _volumes.Count;

        private LastCameraData _lastData;

        private Vector3 LateOffset { get; set; }
        
        private readonly ActorBase _actor;

        public CameraStateMachine(Camera camera, Transform transform, ActorBase actor, ActorCamera master)
        {
            _volumes = new();
            
            Camera = camera;
            Transform = transform;
            _actor = actor;
            Master = actor.Camera;

            FOV = BaseFov;

            OnStateEarlyAssign += _ => RememberRelativeLastData();
            
            DefaultDirection = GetCameraDirection();
            ActualDirection = DefaultDirection;

            blendFactor = 1f;
            interpolatedBlendFactor = 1f;
            
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
            
            if (CurrentState is CameraState currentCameraState)
            {
                Vector3 pos = currentCameraState.StatePosition;
                Quaternion rot = currentCameraState.StateRotation;

                Vector3 center = ActorPosition;
                Vector3 diff = pos - center;
                Position = Vector3.Slerp(_lastData.position, diff, interpolatedBlendFactor);
                Position += center;

                Rotation = Quaternion.Lerp(_lastData.rotation, rot, interpolatedBlendFactor);
                FOV = Mathf.Lerp(_lastData.fov, currentCameraState.StateFOV, interpolatedBlendFactor);
            }
            
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
            bool isExit = IsExact<NewModernState>();
            if (CurrentData != null)
            {
                PanData baseData = CurrentData;
                float enterTime = baseData.easeTimeEnter;
                float exitTime = baseData.easeTimeExit;
                float easeTime = !isExit ? enterTime : exitTime;

                if (easeTime > 0)
                {
                    blendFactor += dt / easeTime;
                }
                else
                {
                    blendFactor = 1f;
                }
                
                blendFactor = Mathf.Clamp01(blendFactor);
                interpolatedBlendFactor = Easings.Get(Easing.Gens, blendFactor);

                if (baseData.allowRotation)
                {
                    Vector2 v = _actor.Input.lookVector;
                    v = Vector3.ClampMagnitude(v, 2f);
                    PanLookOffset = Vector3.Lerp(PanLookOffset, v, 4f * dt);
                }
            }
            else
            {
                PanLookOffset = Vector3.Lerp(PanLookOffset, Vector3.zero, 12f * dt);
            }

            if (isExit)
            {
                if (blendFactor >= 1f)
                {
                    CurrentData = null;
                }
            }
        }
        
        public void RegisterVolume(ChangeCameraVolume vol)
        {
            if (!_volumes.Contains(vol))
            {
                _volumes.Add(vol);
                
                if (CurrentState is not CameraAnimState)
                {
                    ApplyTop();
                }
            }
        }

        public void UnregisterVolume(ChangeCameraVolume vol)
        {
            if (_volumes.Contains(vol))
            {
                _volumes.Remove(vol);
                
                if (CurrentState is not CameraAnimState)
                {
                    ApplyTop();
                }
            }
        }

        public void ApplyTop()
        {
            var top = _volumes.OrderByDescending(v => v.Priority).FirstOrDefault();
            if (top == _lastTop) return;
            _lastTop = top;
            ResetBlendFactor();
            if (top != null) top.Target.SetPan(_actor);
            else
            {
                SetState<NewModernState>();
            }
        }
        
        private Vector3 GetActorWorldPosition()
        {
            Vector3 basePosition = _actor.transform.position + LateOffset;

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

        public void ResetBlendFactor()
        {
            blendFactor = 0f;
            interpolatedBlendFactor = 0f;
        }

        public void SetDirection(Vector3 forward, bool resetY = false)
        {
            Quaternion dir = Quaternion.LookRotation(forward).normalized;
            Yaw = dir.eulerAngles.y;
            Pitch = !resetY ? dir.eulerAngles.x : 0f;
            
            YawAuto = 0f;
            PitchAuto = 0f;
        }

        public void SetDirection(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = pitch;
        }

        public void SetLateOffset(Vector3 offset)
        {
            LateOffset = offset;
        }

        public Vector3 GetOffset()
        {
            Vector3 lookOffset = LookOffset;
            Vector3 globalVerticalOffset = new Vector3(0, lookOffset.y, 0);
            Vector3 cameraSpaceSideOffset = new Vector3(lookOffset.x, 0, lookOffset.z);
            cameraSpaceSideOffset = Transform.TransformDirection(cameraSpaceSideOffset);
            return globalVerticalOffset + cameraSpaceSideOffset + Transform.TransformDirection(PanLookOffset);
        }

        public void ClearVolumes() => _volumes.Clear();

        private void RememberRelativeLastData()
        {
            Vector3 center = ActorPosition;
            _lastData = new LastCameraData
            {
                position = Position - center,
                rotation = Rotation,
                fov = Camera.fieldOfView,
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