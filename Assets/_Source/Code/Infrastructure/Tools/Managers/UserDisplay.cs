using System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
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
            
            Data.Fullscreen.Changed += (_, _) => Apply();
            Data.AntiAliasingQuality.Changed += (_, _) => Apply();
            Data.Sharpness.Changed += (_, _) => Apply();
            Data.UpscaleMode.Changed += (_, _) => Apply();
            Data.UpscaleQuality.Changed += (_, _) => Apply();
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
            Data.AntiAliasingQuality.Value = level;
        }

        public void SetSharpness(float value)
        {
            Data.Sharpness.Value = Mathf.Clamp(value, 0, 2);
        }

        public void SetUpscalingMode(UpscalingMode mode)
        {
            Data.UpscaleMode.Value = mode;
        }

        public void SetUpscalingQuality(UpscalingQuality quality)
        {
            Data.UpscaleQuality.Value = quality;
        }

        public void SetFullscreen(bool value)
        {
            Data.Fullscreen.Value = value;
        }

        public void Apply()
        {
            _hdCameraData.TAAQuality = (HDAdditionalCameraData.TAAQualityLevel)Data.AntiAliasingQuality.Value;
            switch (Data.AntiAliasingQuality.Value)
            {
                case AntiAliasingQuality.Low or AntiAliasingQuality.Medium:
                    _hdCameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.LowQuality;
                    break;
                case AntiAliasingQuality.High:
                    _hdCameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.PostSharpen;
                    break;
            }

            _hdCameraData.taaSharpenStrength = Data.Sharpness.Value;
            _hdCameraData.deepLearningSuperSamplingSharpening = Data.Sharpness.Value;
            _hdCameraData.fidelityFX2SuperResolutionEnableSharpening = true;
            _hdCameraData.fidelityFX2SuperResolutionSharpening = Data.Sharpness.Value;
            _hdCameraData.allowDynamicResolution = Data.UpscaleMode.Value != UpscalingMode.TAA;

            _hdCameraData.deepLearningSuperSamplingUseCustomAttributes = false;
            _hdCameraData.deepLearningSuperSamplingUseCustomQualitySettings = false;
            _hdCameraData.deepLearningSuperSamplingUseOptimalSettings = false;

            _hdCameraData.fidelityFX2SuperResolutionUseCustomAttributes = false;
            _hdCameraData.fidelityFX2SuperResolutionUseCustomQualitySettings = false;
            _hdCameraData.fidelityFX2SuperResolutionUseOptimalSettings = false;

            switch (Data.UpscaleMode.Value)
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

            switch (Data.UpscaleQuality.Value)
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

            Screen.SetResolution(Screen.width, Screen.height, Data.Fullscreen.Value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        }
    }

    [Serializable]
    public class UserDisplaySettings
    {
        public ReactiveVar<bool> Fullscreen = new(true);
        public ReactiveVar<AntiAliasingQuality> AntiAliasingQuality = new(Managers.AntiAliasingQuality.High);
        public ReactiveVar<float> Sharpness = new(0.25f);
        public ReactiveVar<UpscalingMode> UpscaleMode = new(UpscalingMode.TAA);
        public ReactiveVar<UpscalingQuality> UpscaleQuality = new(UpscalingQuality.Native);
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