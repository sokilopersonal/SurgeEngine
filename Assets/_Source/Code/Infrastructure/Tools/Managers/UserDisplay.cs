using System;
using SurgeEngine._Source.Code.Infrastructure.Tools.Services;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers
{
    public class UserDisplay : JsonStorageService<UserDisplaySettings>, IInitializable, ILateDisposable
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
                Initialize();
            }
        }

        public void Initialize()
        {
            SetupCamera();
            
            Apply();
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

        public void SetUpscalingMode(UpscalingMode mode)
        {
            Data.upscaleMode = mode;
           
        }

        public void SetUpscalingQuality(UpscalingQuality quality)
        {
            Data.upscaleQuality = quality;
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
            _hdCameraData.allowDynamicResolution = true;
            
            _hdCameraData.deepLearningSuperSamplingUseCustomAttributes = false;
            _hdCameraData.deepLearningSuperSamplingUseCustomQualitySettings = false;
            _hdCameraData.deepLearningSuperSamplingUseOptimalSettings = false;

            _hdCameraData.fidelityFX2SuperResolutionUseCustomAttributes = false;
            _hdCameraData.fidelityFX2SuperResolutionUseCustomQualitySettings = false;
            _hdCameraData.fidelityFX2SuperResolutionUseOptimalSettings = false;
            
            switch (Data.upscaleMode)
            {
                case UpscalingMode.TAA:
                    _hdCameraData.allowDeepLearningSuperSampling = false;
                    _hdCameraData.allowFidelityFX2SuperResolution = false;
                    break;
                case UpscalingMode.FSR:
                    _hdCameraData.allowFidelityFX2SuperResolution = true;
                    _hdCameraData.allowDeepLearningSuperSampling = false;
                    break;
                case UpscalingMode.DLSS:
                    _hdCameraData.allowDeepLearningSuperSampling = true;
                    _hdCameraData.allowFidelityFX2SuperResolution = false;
                    break;
            }

            switch (Data.upscaleQuality)
            {
                case UpscalingQuality.UltraPerformance:
                    DynamicResolutionHandler.SetDynamicResScaler(() => 33.3333f, DynamicResScalePolicyType.ReturnsPercentage);
                    break;
                case UpscalingQuality.Performance:
                    DynamicResolutionHandler.SetDynamicResScaler(() => 50f, DynamicResScalePolicyType.ReturnsPercentage);
                    break;
                case UpscalingQuality.Balanced:
                    DynamicResolutionHandler.SetDynamicResScaler(() => 57.88888f, DynamicResScalePolicyType.ReturnsPercentage);
                    break;
                case UpscalingQuality.Quality:
                    DynamicResolutionHandler.SetDynamicResScaler(() => 66.666667f, DynamicResScalePolicyType.ReturnsPercentage);
                    break;
                case UpscalingQuality.Native:
                    DynamicResolutionHandler.SetDynamicResScaler(() => 100f, DynamicResScalePolicyType.ReturnsPercentage);
                    break;
            }
            
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
        public UpscalingMode upscaleMode = UpscalingMode.TAA;
        public UpscalingQuality upscaleQuality = UpscalingQuality.Native;
    }
    
    public enum AntiAliasingQuality
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum UpscalingMode
    {
        TAA,
        DLSS,
        FSR,
    }

    public enum UpscalingQuality
    {
        UltraPerformance,
        Performance,
        Balanced,
        Quality,
        Native
    }
}