using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem
{
    public class CameraStateMachine : FStateMachine, IPointMarkerLoader
    {
        public CharacterCamera Master { get; }
        public Camera Camera { get; }
        public Transform Transform { get; }

        public float Yaw { get; set; }
        public float Pitch { get; set; }

        public float BaseFov { get; private set; }

        private Vector3 _position;
        private Vector3 _characterPosition;
        private Quaternion _rotation;
        private float _fovY;

        public PanData CurrentData { get; set; }
        public float BlendFactor { get; private set; }

        private bool _is2dCamera;
        
        private readonly Stack<ChangeCameraVolume> _volumes;
        private ChangeCameraVolume _lastTop;
        public int VolumeCount => _volumes.Count;
        public ChangeCameraVolume Top => _volumes.Count > 0 ? _volumes.Peek() : null;

        private CameraData _data;
        
        private readonly CharacterBase _character;

        public CameraStateMachine(Camera camera, Transform transform, CharacterBase character)
        {
            _volumes = new();
            
            Camera = camera;
            Transform = transform;
            _character = character;
            Master = character.Camera;
            
            BaseFov = Camera.fieldOfView;
            _fovY = BaseFov;

            OnStateEarlyAssign += _ => RememberRelativeData();
            _character.Kinematics.OnPath2DChange += Set2DCamera;

            CompleteBlend();
        }

        public override void Tick(float dt)
        {
            _characterPosition = _character.transform.position;
            
            base.Tick(dt);
            
            Blend();
            UpdateBlendFactor();

            Transform.position = _position;
            Transform.rotation = _rotation;
            Camera.fieldOfView = _fovY;
        }

        private void UpdateBlendFactor()
        {
            bool isExit = VolumeCount == 0;
            if (CurrentData != null)
            {
                PanData baseData = CurrentData;
                float enterTime = baseData.easeTimeEnter;
                float exitTime = baseData.easeTimeExit;
                float easeTime = !isExit ? enterTime : exitTime;

                if (easeTime > 0)
                {
                    BlendFactor += Time.deltaTime / easeTime;
                }
                else
                {
                    BlendFactor = 1f;
                }
                
                BlendFactor = Mathf.Clamp01(BlendFactor);
            }
        }

        private void Blend()
        {
            if (CurrentState is CameraState currentCameraState)
            {
                Vector3 pos = currentCameraState.StatePosition;
                Quaternion rot = currentCameraState.StateRotation;
                float t = Easings.Get(Easing.Gens, BlendFactor);

                if (BlendFactor < 1)
                {
                    Vector3 center = _characterPosition;
                    Vector3 diff = pos - center;
                    _position = Vector3.Lerp(_data.position, diff, t);
                    _position += center;
                    
                    _rotation = Quaternion.Lerp(_data.rotation, rot, t);
                    _fovY = Mathf.Lerp(_data.fov, currentCameraState.StateFOV, t);
                }
                
                if (BlendFactor >= 1)
                {
                    _position = pos;
                    _rotation = rot;
                    _fovY = currentCameraState.StateFOV;
                }
            }
        }

        protected override void EnterState(FState newState)
        {
            if (_is2dCamera)
            {
                var type = newState.GetType();
                if (type == typeof(NewModernState))
                {
                    if (states.TryGetValue(typeof(Camera2DState), out var value))
                    {
                        newState = value;
                    }
                }
            }
            
            base.EnterState(newState);
        }

        public void RegisterVolume(ChangeCameraVolume vol)
        {
            if (!_volumes.Contains(vol) && vol.Target)
            {
                var tempList = new List<ChangeCameraVolume>(_volumes) { vol };
                tempList = tempList.OrderBy(v => v.Priority).ToList();

                _volumes.Clear();
                foreach (var t in tempList)
                {
                    _volumes.Push(t);
                }
                
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
                var tempList = new List<ChangeCameraVolume>(_volumes);
                tempList.Remove(vol);
                tempList = tempList.OrderBy(v => v.Priority).ToList();

                _volumes.Clear();
                for (int i = 0; i < tempList.Count; i++)
                {
                    _volumes.Push(tempList[i]);
                }

                if (CurrentState is not CameraAnimState)
                {
                    ApplyTop();
                }
            }
        }

        public void ApplyTop()
        {
            var top = Top;
            if (top == _lastTop) return;
            _lastTop = top;

            ResetBlendFactor();

            if (top != null) top.Target.SetPan(_character);
            else
            {
                SetState<NewModernState>();
            }
        }

        private void Set2DCamera(ChangeMode2DData data)
        {
            if (data != null && data.IsCameraChange)
            {
                _is2dCamera = true;
                ResetBlendFactor();
                CurrentData = new();
                SetState<Camera2DState>();
            }
            else if (data == null)
            {
                if (CurrentState is Camera2DState)
                {
                    _is2dCamera = false;
                    ResetBlendFactor();
                    SetDirection(_character.transform.forward);
                    CurrentData = new();
                    SetState<NewModernState>();
                }
            }
        }

        public void CompleteBlend() => BlendFactor = 1f;
        
        public void ResetBlendFactor()
        {
            BlendFactor = 0f;
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
            Vector3 center = _characterPosition;
            _data = new()
            {
                position = _position - center,
                rotation = _rotation,
                fov = Camera.fieldOfView,
            };
        }

        public void Load()
        {
            if (VolumeCount == 0)
            {
                SetState<NewModernState>();
                _lastTop = null;
            }
            else
            {
                ApplyTop();
            }
            
            if (CurrentState is CameraState state)
            {
                var pos = state.StatePosition;
                var rot = state.StateRotation;
                var fov = state.StateFOV;
                
                _position = pos;
                _rotation = rot;
                _fovY = fov;
                
                _data = new()
                {
                    position = pos - _characterPosition,
                    rotation = rot,
                    fov = fov,
                };

                BlendFactor = 1;
            }
        }
    }

    public class CameraData
    {
        public Vector3 position;
        public Quaternion rotation;
        public float fov;
    }
}