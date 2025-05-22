using System;
using System.Collections.Generic;
using SurgeEngine.Code.Gameplay.CommonObjects.Lighting;
using SurgeEngine.Code.Infrastructure.Tools.Services;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers
{
    public class UserGraphics : JsonStorageService<GraphicsData>, ILateDisposable
    {
        private readonly VolumeProfile _volume;
        private readonly List<LightDefiner> _lightsData;
        private HDAdditionalCameraData _hdCameraData;
        
        private const int MaxTextureQuality = 3;
        
        private readonly string[] _refractionQualityKeywords =
        {
            "_REFRACTIONQUALITY_LOW",
            "_REFRACTIONQUALITY_MEDIUM", 
            "_REFRACTIONQUALITY_NATIVE",
        };
        
        public UserGraphics(VolumeProfile profile)
        {
            _volume = profile;
            _lightsData = new List<LightDefiner>();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void LateDispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Additive)
            {
                SetupCamera();
                
                Apply();
            }
        }

        public void SetTextureQuality(TextureQuality value)
        {
            Data.textureQuality = value;
        }

        public void SetSunShadowsQuality(ShadowsQuality value)
        {
            Data.sunShadowsQuality = value;
        }

        public void SetAdditionalShadowsQuality(ShadowsQuality value)
        {
            Data.additionalShadowsQuality = value;
        }

        public void SetBloomQuality(BloomQuality value)
        {
            Data.bloomQuality = value;
        }

        public void SetAmbientOcclusionQuality(AmbientOcclusionQuality value)
        {
            Data.aoQuality = value;
        }

        public void SetMotionBlurQuality(MotionBlurQuality value)
        {
            Data.motionBlurQuality = value;
        }

        public void SetRefractionQuality(RefractionQuality level)
        {
            Data.refractionQuality = level;
        }

        public void SetScreenSpaceReflectionsQuality(ScreenSpaceReflectionQuality level)
        {
            Data.screenSpaceReflectionQuality = level;
        }

        public void SetContactShadows(ContactShadowsQuality level)
        {
            Data.contactShadowsQuality = level;
        }

        public void SetSubSurfaceScattering(SubSurfaceScatteringQuality level)
        {
            Data.subSurfaceScatteringQuality = level;
        }

        public void AddLight(LightDefiner data)
        {
            _lightsData.Add(data);
        }

        public void RemoveLight(LightDefiner data)
        {
            _lightsData.Remove(data);
        }

        private static void SetKeyword(string[] keys, int value)
        {
            string key = keys[value];
            foreach (var keyword in keys)
            {
                Shader.DisableKeyword(keyword);
            }
            
            Shader.EnableKeyword(key);
        }

        public void Apply()
        {
            // Texture quality
            QualitySettings.globalTextureMipmapLimit = MaxTextureQuality - (int)Data.textureQuality;
            
            // Sun Shadows
            foreach (var light in _lightsData)
            {
                if (light.Component.type == LightType.Directional)
                {
                    var data = light.Data;
                    data.shadowResolution.level = (int)Data.sunShadowsQuality;
                }
            }
            
            // Additional Shadows
            foreach (var light in _lightsData)
            {
                if (light.Component.type == LightType.Directional)
                {
                    continue;
                }
                
                var data = light.Data;
                data.shadowResolution.level = (int)Data.additionalShadowsQuality;
            }
            
            // Bloom Quality
            if (_volume.TryGet(out Bloom bloom))
            {
                if (Data.bloomQuality == BloomQuality.Off)
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Bloom, false);
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Bloom, true);
                    bloom.quality.value = (int)Data.bloomQuality - 1;
                }
            }
            
            // GTAO Quality
            if (_volume.TryGet(out ScreenSpaceAmbientOcclusion ssao))
            {
                if (Data.aoQuality == AmbientOcclusionQuality.Off)
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, false);
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, true);
                    ssao.quality.value = (int)Data.aoQuality - 1;
                }
            }
            
            // Motion Blur Quality
            if (_volume.TryGet(out MotionBlur motionBlur))
            {
                if (Data.motionBlurQuality == MotionBlurQuality.Off)
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, false);
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, true);
                    motionBlur.quality.value = (int)Data.motionBlurQuality - 1;
                }
            }
            
            // SSR Quality
            if (_volume.TryGet(out ScreenSpaceReflection ssr))
            {
                if (Data.screenSpaceReflectionQuality == ScreenSpaceReflectionQuality.Off)
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSR, false);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSRAsync, false);
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSR, true);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSRAsync, true);
                    ssr.quality.value = (int)Data.screenSpaceReflectionQuality - 1;
                }
            }
            
            // Contact Shadows Quality
            if (_volume.TryGet(out ContactShadows contactShadows))
            {
                if (Data.contactShadowsQuality == ContactShadowsQuality.Off)
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadows, false);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadowsAsync, false);
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadows, true);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadowsAsync, true);
                    contactShadows.quality.value = (int)Data.contactShadowsQuality - 1;
                }
            }

            if (Data.subSurfaceScatteringQuality == SubSurfaceScatteringQuality.Off)
            {
                _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Transmission, false);
                _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SubsurfaceScattering, false);
            }
            else
            {
                _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Transmission, true);
                _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SubsurfaceScattering, true);
            }
            
            // Refraction Quality
            SetKeyword(_refractionQualityKeywords, (int)Data.refractionQuality);
        }

        private void SetupCamera()
        {
            var camera = Camera.main;
            if (camera != null)
            {
                _hdCameraData = camera.GetComponent<HDAdditionalCameraData>();
                _hdCameraData.customRenderingSettings = true;
            }
        }
    }

    [Serializable]
    public class GraphicsData
    {
        public ShadowsQuality sunShadowsQuality = ShadowsQuality.High;
        public ShadowsQuality additionalShadowsQuality = ShadowsQuality.High;
        public BloomQuality bloomQuality = BloomQuality.High;
        public AmbientOcclusionQuality aoQuality = AmbientOcclusionQuality.High;
        public MotionBlurQuality motionBlurQuality = MotionBlurQuality.High;
        public TextureQuality textureQuality = TextureQuality.High;
        public RefractionQuality refractionQuality = RefractionQuality.Native;
        public ScreenSpaceReflectionQuality screenSpaceReflectionQuality = ScreenSpaceReflectionQuality.Medium;
        public ContactShadowsQuality contactShadowsQuality = ContactShadowsQuality.Medium;
        public SubSurfaceScatteringQuality subSurfaceScatteringQuality = SubSurfaceScatteringQuality.On;
    }
    
    public enum TextureQuality
    {
        Low = 0,
        Medium = 1,
        High = 2,
        VeryHigh = 3,
    }

    public enum ShadowsQuality
    {
        Low = 0,
        Medium = 1,
        High = 2,
        VeryHigh = 3,
    }

    public enum BloomQuality
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum AmbientOcclusionQuality
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum MotionBlurQuality
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum RefractionQuality
    {
        Low = 0,
        Medium = 1,
        Native = 2,
    }
    
    public enum ScreenSpaceReflectionQuality
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3,
    }

    public enum MaterialQuality
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum ContactShadowsQuality
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum SubSurfaceScatteringQuality
    {
        Off = 0,
        On = 1
    }
}