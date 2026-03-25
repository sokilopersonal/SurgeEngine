using System;
using System.Collections.Generic;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem
{
    public class CameraVolumeStack
    {
        private readonly List<ChangeCameraVolume> _volumes = new();
        private ChangeCameraVolume _lastTop;
        private bool _hasNotified;

        public int Count => _volumes.Count;
        public ChangeCameraVolume Top => _volumes.Count > 0 ? _volumes[^1] : null;

        public event Action<ChangeCameraVolume> OnTopChanged;

        public void Register(ChangeCameraVolume vol)
        {
            if (_volumes.Contains(vol) || !vol.Target) return;

            _volumes.Add(vol);
            _volumes.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            NotifyTopChanged();
        }

        public void Unregister(ChangeCameraVolume vol)
        {
            if (!_volumes.Remove(vol)) return;

            _volumes.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            NotifyTopChanged();
        }

        public void Clear() => _volumes.Clear();

        private void NotifyTopChanged()
        {
            var top = Top;
            if (_hasNotified && top == _lastTop) return;
            _lastTop = top;
            _hasNotified = true;
            OnTopChanged?.Invoke(top);
        }

        public void ResetLastTop()
        {
            _lastTop = null;
            _hasNotified = false;
        }
    }
}