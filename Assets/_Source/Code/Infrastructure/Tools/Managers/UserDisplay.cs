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

        public void SetResolution(Vector2 value)
        {
            Data.screenWidth = (int)value.x;
            Data.screenHeight = (int)value.y;
        }

        public void SetFullscreen(bool value)
        {
            Data.fullscreen = value;
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
            
            Screen.SetResolution(Data.screenWidth, Data.screenHeight, Data.fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        }
    }

    [Serializable]
    public class UserDisplaySettings
    {
        public int screenWidth = 1920;
        public int screenHeight = 1080;
        public bool fullscreen = true;
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