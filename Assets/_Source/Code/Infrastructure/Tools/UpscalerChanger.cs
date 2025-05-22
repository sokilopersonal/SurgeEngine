
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

#if ENABLE_NVIDIA_DLSS
using UnityEngine.NVIDIA;
#endif

namespace SurgeEngine.Code.Infrastructure.Tools
{
    public enum Upscaler
    {
        TAAU,
        DLSS,
        FSR,
        STP
    }

    public enum UpscalingMode
    {
        Native,
        Quality,
        Balanced,
        Performance,
        UltraPerformance
    }
    
    public class UpscalerChanger
    {
        public Upscaler Upscaler { get; private set; } = Upscaler.TAAU;
        public UpscalingMode UpscalingMode { get; set; } = UpscalingMode.Quality;

        private Camera _camera;
        private HDAdditionalCameraData _data;
        
        public UpscalerChanger()
        {
            DynamicResolutionHandler.SetDynamicResScaler(delegate()
            {
                switch (UpscalingMode)
                {
                    case UpscalingMode.Native:
                        return 100f;
                    case UpscalingMode.Quality:
                        return 66f;
                    case UpscalingMode.Balanced:
                        return 58f;
                    case UpscalingMode.Performance:
                        return 50f;
                    case UpscalingMode.UltraPerformance:
                        return 33f;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }, DynamicResScalePolicyType.ReturnsPercentage);
        }

        public void SetCameraAndData(Camera camera, HDAdditionalCameraData data)
        {
            _camera = camera;
            _data = data;
            _data.allowDynamicResolution = true;
        }

        public Upscaler Set(Upscaler upscaler)
        {
            Upscaler = upscaler;
            
            switch (upscaler)
            {
                case Upscaler.DLSS:
                    _data.allowDeepLearningSuperSampling = true;
                    _data.allowFidelityFX2SuperResolution = false;
                    break;
                case Upscaler.FSR:
                    _data.allowDeepLearningSuperSampling = false;
                    _data.allowFidelityFX2SuperResolution = true;
                    break;
                case Upscaler.STP:
                    _data.allowDeepLearningSuperSampling = false;
                    _data.allowFidelityFX2SuperResolution = false;
                    break;
            }

            return Upscaler;
        }

        public UpscalingMode SetQuality(UpscalingMode mode)
        {
            UpscalingMode = mode;

            switch (mode)
            {
                case UpscalingMode.Native:
                    break;
                case UpscalingMode.Quality:
                    _data.deepLearningSuperSamplingQuality = 2;
                    _data.fidelityFX2SuperResolutionQuality = 2;
                    break;
                case UpscalingMode.Balanced:
                    _data.deepLearningSuperSamplingQuality = 1;
                    _data.fidelityFX2SuperResolutionQuality = 1;
                    break;
                case UpscalingMode.Performance:
                    _data.deepLearningSuperSamplingQuality = 0;
                    _data.fidelityFX2SuperResolutionQuality = 0;
                    break;
                case UpscalingMode.UltraPerformance:
                    _data.deepLearningSuperSamplingQuality = 3;
                    _data.fidelityFX2SuperResolutionQuality = 3;
                    break;
            }
            
            return UpscalingMode;
        }
    }
}