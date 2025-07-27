using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SurgeEngine.Code.Core.Actor.CameraSystem
{
    public class CameraStateMachine : FStateMachine, IPointMarkerLoader
    {
        public ActorCamera Master { get; }
        public Camera Camera { get; }
        public Transform Transform { get; }

        public float Yaw { get; set; }
        public float Pitch { get; set; }

        public float BaseFov => 50f;

        private Vector3 _position;
        private Quaternion _rotation;
        private float _fovY;
        private Vector3 _actorPosition;

        public PanData CurrentData { get; set; }
        private float _blendFactor;
        private float _interpolatedBlendFactor;
        
        private readonly List<ChangeCameraVolume> _volumes;
        private ChangeCameraVolume _lastTop;
        public int VolumeCount => _volumes.Count;

        private CameraData _data;
        
        private readonly ActorBase _actor;

        public CameraStateMachine(Camera camera, Transform transform, ActorBase actor)
        {
            _volumes = new();
            
            Camera = camera;
            Transform = transform;
            _actor = actor;
            Master = actor.Camera;

            _fovY = BaseFov;

            OnStateEarlyAssign += _ => RememberRelativeData();

            _blendFactor = 1f;
            _interpolatedBlendFactor = 1f;
        }

        public override void Tick(float dt)
        {
            PanBlend(dt);
            
            _actorPosition = _actor.transform.position;

            base.Tick(dt);
            
            if (CurrentState is CameraState currentCameraState)
            {
                Vector3 pos = currentCameraState.StatePosition;
                Quaternion rot = currentCameraState.StateRotation;

                if (_blendFactor < 1)
                {
                    Vector3 center = _actorPosition;
                    Vector3 diff = pos - center;
                    _position = Vector3.Slerp(_data.position, diff, _interpolatedBlendFactor);
                    _position += center;
                    
                    _rotation = Quaternion.Lerp(_data.rotation, rot, _interpolatedBlendFactor);
                    _fovY = Mathf.Lerp(_data.fov, currentCameraState.StateFOV, _interpolatedBlendFactor);
                }
                
                if (_blendFactor >= 1)
                {
                    _position = pos;
                    _rotation = rot;
                    _fovY = currentCameraState.StateFOV;
                }
            }
            
            Transform.position = _position;
            Transform.rotation = _rotation;
            Camera.fieldOfView = _fovY;
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
                    _blendFactor += dt / easeTime;
                }
                else
                {
                    _blendFactor = 1f;
                }
                
                _blendFactor = Mathf.Clamp01(_blendFactor);
                _interpolatedBlendFactor = Easings.Get(Easing.Gens, _blendFactor);
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
            else SetState<NewModernState>();
        }
        
        public void ResetBlendFactor()
        {
            _blendFactor = 0f;
            _interpolatedBlendFactor = 0f;
        }

        public void SetDirection(Vector3 forward, bool resetY = false)
        {
            Quaternion dir = Quaternion.LookRotation(forward).normalized;
            Yaw = dir.eulerAngles.y;
            Pitch = !resetY ? dir.eulerAngles.x : 0f;
        }

        public void SetDirection(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = pitch;
        }

        public void ClearVolumes() => _volumes.Clear();

        private void RememberRelativeData()
        {
            Vector3 center = _actorPosition;
            _data = new CameraData
            {
                position = _position - center,
                rotation = _rotation,
                fov = Camera.fieldOfView,
            };
        }

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            
        }
    }

    public class CameraData
    {
        public Vector3 position;
        public Quaternion rotation;
        public float fov;
    }
}