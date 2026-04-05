using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem
{
    public class CameraStateMachine : FStateMachine, IPointMarkerLoader
    {
        public CharacterCamera Master { get; }
        private Camera Camera { get; }
        public Transform Transform { get; }

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float BaseFov { get; }

        public CameraEaseData EaseData { get; set; }
        public float BlendFactor => _blending.BlendFactor;
        public ChangeVolumeCamera Top => _volumeStack.Top;
        public bool IsExiting { get; set; }
        private int VolumeCount => _volumeStack.Count;

        private Vector3 _position;
        private Vector3 _characterPosition;
        private Quaternion _rotation;
        private float _fovY;
        private bool _is2dCamera;

        private readonly CameraBlending _blending = new();
        private readonly CameraVolumeStack _volumeStack = new();
        private readonly CharacterBase _character;
        
        public CameraBlending Blending => _blending;

        public CameraStateMachine(Camera camera, Transform transform, CharacterBase character)
        {
            Camera = camera;
            Transform = transform;
            _character = character;
            Master = character.Camera;
            BaseFov = Camera.fieldOfView;
            _fovY = BaseFov;

            OnStateEarlyAssign += _ => RememberRelativeData();
            _character.Kinematics.Mode.OnPath2DChange += Set2DCamera;
            _volumeStack.OnTopChanged += HandleTopChanged;
        }
        
        public void Initialize()
        {
            var animState = GetState<CameraAnimState>();
            if (animState != null)
                animState.OnFinished += HandleAnimFinished;
            
            _blending.Complete();
        }
        
        private void HandleAnimFinished(CameraAnimState state)
        {
            _blending.Reset();

            if (VolumeCount == 0)
            {
                var startData = _character.GetStartData();
                float exit = startData.startType == StartType.Standing ? 0.5f : 0f;
                EaseData = new CameraEaseData(0, exit);
                IsExiting = true;
                
                SetState<NewModernState>();
            }
            else
            {
                var top = Top;
                _volumeStack.ResetLastTop();
                top.Target.SetPan(_character, CameraEaseData.FromVolume(top));
            }
            
            state.OnFinished -= HandleAnimFinished;
        }

        public override void Tick(float dt)
        {
            _characterPosition = _character.transform.position;
            base.Tick(dt);

            if (CurrentState is CameraState cam)
            {
                _blending.Tick(dt, EaseData, IsExiting);
                (_position, _rotation, _fovY) = _blending.Evaluate(
                    cam.StatePosition, cam.StateRotation, cam.StateFOV, _characterPosition);
            }

            Transform.position = _position;
            Transform.rotation = _rotation;
            Camera.fieldOfView = _fovY;
        }

        protected override void EnterState(FState newState)
        {
            if (_is2dCamera && newState.GetType() == typeof(NewModernState))
            {
                if (states.TryGetValue(typeof(Camera2DState), out var camera2DState))
                    newState = camera2DState;
            }

            base.EnterState(newState);
        }

        public void RegisterVolume(ChangeVolumeCamera vol)
        {
            _volumeStack.Register(vol);
        }

        public void UnregisterVolume(ChangeVolumeCamera vol)
        {
            _volumeStack.Unregister(vol);
        }

        private void HandleTopChanged(ChangeVolumeCamera top)
        {
            if (CurrentState is CameraAnimState) return;
            _blending.Reset();

            if (top != null)
            {
                IsExiting = false;
                top.Target.SetPan(_character, CameraEaseData.FromVolume(top));
            }
            else
            {
                IsExiting = true;
                SetState<NewModernState>();
            }
        }

        private void Set2DCamera(ChangeMode2DData data)
        {
            if (data != null && data.IsCameraChange)
            {
                _is2dCamera = true;
                _blending.Reset();
                EaseData = new CameraEaseData(1, 1);
                SetState<Camera2DState>();
            }
            else if (data == null && _is2dCamera)
            {
                _is2dCamera = false;
                _blending.Reset();
                SetDirection(_character.transform.forward);
                SetState<NewModernState>();
            }
        }

        public void SetDirection(Vector3 forward, bool resetPitch = false)
        {
            Quaternion dir = Quaternion.LookRotation(forward).normalized;
            Yaw = dir.eulerAngles.y;
            Pitch = resetPitch ? 0f : dir.eulerAngles.x;
        }

        public void SetDirection(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = pitch;
        }

        public void ClearVolumes() => _volumeStack.Clear();

        private void RememberRelativeData()
        {
            _blending.RememberFrom(
                _position - _characterPosition,
                _rotation,
                Camera.fieldOfView);
        }

        public void Load()
        {
            if (VolumeCount == 0)
            {
                SetState<NewModernState>();
                _volumeStack.ResetLastTop();
            }
            else
            {
                HandleTopChanged(Top);
            }

            if (CurrentState is CameraState state)
            {
                _position = state.StatePosition;
                _rotation = state.StateRotation;
                _fovY = state.StateFOV;

                _blending.RememberFrom(
                    _position - _characterPosition,
                    _rotation,
                    _fovY);

                _blending.Complete();
            }
        }
    }
}