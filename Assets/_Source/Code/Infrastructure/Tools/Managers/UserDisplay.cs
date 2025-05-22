using System;
using SurgeEngine.Code.Infrastructure.Tools.Services;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers
{
    public class UserDisplay : JsonStorageService<UserDisplaySettings>, ILateDisposable
    {
        private HDAdditionalCameraData _hdCameraData;

        public UserDisplay()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void LateDispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg1 != LoadSceneMode.Additive)
            {
                SetupCamera();

                Apply();
            }
        }

        private void SetupCamera()
        {
            var camera = Camera.main;
            if (camera != null)
            {
                _hdCameraData = camera.GetComponent<HDAdditionalCameraData>();
            }
        }
        
        public void SetAntiAliasing(AntiAliasingQuality level)
        {
            Data.antiAliasingQuality = level;
        }

        public void SetSharpness(float value)
        {
            Data.sharpness = Mathf.Clamp(value, 0, 2);
        }

        public void Apply()
        {
            _hdCameraData.TAAQuality = (HDAdditionalCameraData.TAAQualityLevel)Data.antiAliasingQuality;
            switch (Data.antiAliasingQuality)
            {
                case AntiAliasingQuality.Low:
                    _hdCameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.LowQuality;
                    break;
                case AntiAliasingQuality.Medium or AntiAliasingQuality.High:
                    _hdCameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.PostSharpen;
                    break;
            }
            
            _hdCameraData.taaSharpenStrength = Data.sharpness;
        }
    }

    [Serializable]
    public class UserDisplaySettings
    {
        public AntiAliasingQuality antiAliasingQuality = AntiAliasingQuality.High;
        public float sharpness = 0.25f;
    }
    
    public enum AntiAliasingQuality
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}