using System;
using System.Collections.Generic;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Lighting;
using SurgeEngine._Source.Code.Infrastructure.Tools.Services;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers
{
    public class UserGraphics : JsonStorageService<GraphicsData>, ILateDisposable
    {
        private readonly VolumeProfile _volume;
        private readonly List<LightDefiner> _lightsData;
        private HDAdditionalCameraData _hdCameraData;
        
        private const int MaxTextureQuality = 3;

        private readonly Dictionary<MeshQuality, float> _meshQualityLods = new()
        {
            [MeshQuality.Medium] = 4f,
            [MeshQuality.High] = 2f,
            [MeshQuality.VeryHigh] = 0.5f
        };
        
        public UserGraphics()
        {
            // Create temp profile
            var volumeObject = new GameObject("[UserGraphics] Volume");
            var volumeComponent = volumeObject.AddComponent<Volume>();
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            Object.DontDestroyOnLoad(volumeObject);

            var ao = profile.Add<ScreenSpaceAmbientOcclusion>();
            var ssr = profile.Add<ScreenSpaceReflection>();
            var contactShadows = profile.Add<ContactShadows>();
            var bloom = profile.Add<Bloom>();
            var motionBlur = profile.Add<MotionBlur>();

            ao.quality.overrideState = true;
            ao.rayTracing.value = true;
            ssr.quality.overrideState = true;
            ssr.tracing.overrideState = true;
            ssr.tracing.value = RayCastingMode.RayTracing;
            ssr.rayLength = 450;
            contactShadows.quality.overrideState = true;
            bloom.quality.overrideState = true;
            motionBlur.quality.overrideState = true;
            
            volumeComponent.profile = profile;
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
                Apply();
            }
        }

        public void SetTextureQuality(TextureQuality value)
        {
            Data.textureQuality = value;
        }

        public void SetMeshQuality(MeshQuality value)
        {
            Data.meshQuality = value;
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
        
        public void SetMotionBlur(MotionBlurState value)
        {
            Data.motionBlur = value;
        }

        public void SetMotionBlurQuality(MotionBlurQuality value)
        {
            Data.motionBlurQuality = value;
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
            SetupCamera();
            
            // Texture quality
            QualitySettings.globalTextureMipmapLimit = MaxTextureQuality - (int)Data.textureQuality;
            QualitySettings.meshLodThreshold = _meshQualityLods[Data.meshQuality];
            
            // Sun Shadows
            var sun = RenderSettings.sun;
            if (sun != null)
            {
                var sunData = sun.GetComponent<HDAdditionalLightData>();
                sunData.shadowResolution.level = (int)Data.sunShadowsQuality;
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
                if (Data.motionBlur == MotionBlurState.Off)
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, false);
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, true);
                    motionBlur.quality.value = (int)Data.motionBlurQuality;
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
        public MotionBlurState motionBlur = MotionBlurState.On;
        public MotionBlurQuality motionBlurQuality = MotionBlurQuality.High;
        public TextureQuality textureQuality = TextureQuality.High;
        public MeshQuality meshQuality = MeshQuality.VeryHigh;
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
    
    public enum MeshQuality
    {
        Medium = 0,
        High = 1,
        VeryHigh = 2,
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

    public enum MotionBlurState
    {
        Off = 0,
        On = 1
    }

    public enum MotionBlurQuality
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
    
    public enum ScreenSpaceReflectionQuality
    {
        Off = 0,
        Low = 1,
        Medium = 2,
        High = 3,
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