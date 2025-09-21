using System;
using System.Collections.Generic;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Lighting;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using SurgeEngine._Source.Code.Infrastructure.Tools.Services;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers
{
    public class UserGraphics : JsonStorageService<GraphicsData>, IInitializable, ILateDisposable
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

        public void Initialize()
        {
            Apply();
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
            Data.TextureQuality.Value = value;
            
            Apply();
        }

        public void SetMeshQuality(MeshQuality value)
        {
            Data.MeshQuality.Value = value;
            
            Apply();
        }

        public void SetSunShadowsQuality(ShadowsQuality value)
        {
            Data.SunShadowsQuality.Value = value;
            
            Apply();
        }

        public void SetAdditionalShadowsQuality(ShadowsQuality value)
        {
            Data.AdditionalShadowsQuality.Value = value;
            
            Apply();
        }

        public void SetBloomQuality(BloomQuality value)
        {
            Data.BloomQuality.Value = value;
            
            Apply();
        }

        public void SetAmbientOcclusionQuality(AmbientOcclusionQuality value)
        {
            Data.AOQuality.Value = value;
            
            Apply();
        }

        public void SetMotionBlur(MotionBlurState value)
        {
            Data.MotionBlur.Value = value;
            
            Apply();
        }

        public void SetMotionBlurQuality(MotionBlurQuality value)
        {
            Data.MotionBlurQuality.Value = value;
            
            Apply();
        }

        public void SetScreenSpaceReflectionsQuality(ScreenSpaceReflectionQuality level)
        {
            Data.ScreenSpaceReflectionQuality.Value = level;
            
            Apply();
        }

        public void SetSubSurfaceScattering(SubSurfaceScatteringQuality level)
        {
            Data.SubSurfaceScatteringQuality.Value = level;
            
            Apply();
        }

        public void AddLight(LightDefiner data)
        {
            _lightsData.Add(data);
        }

        public void RemoveLight(LightDefiner data)
        {
            _lightsData.Remove(data);
        }

        public void Apply()
        {
            SetupCamera();
            
            // Texture quality
            QualitySettings.globalTextureMipmapLimit = MaxTextureQuality - (int)Data.TextureQuality.Value;
            QualitySettings.meshLodThreshold = _meshQualityLods[Data.MeshQuality.Value];
            
            // Sun Shadows
            var sun = RenderSettings.sun;
            if (sun != null)
            {
                var sunData = sun.GetComponent<HDAdditionalLightData>();
                sunData.shadowResolution.level = (int)Data.SunShadowsQuality.Value;
            }
            
            // Additional Shadows
            foreach (var light in _lightsData)
            {
                if (light.Component.type == LightType.Directional)
                {
                    continue;
                }
                
                var data = light.Data;
                data.shadowResolution.level = (int)Data.AdditionalShadowsQuality.Value;
            }
            
            // Bloom Quality
            if (_volume.TryGet(out Bloom bloom))
            {
                if (Data.BloomQuality.Value == BloomQuality.Off)
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Bloom, false);
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Bloom, true);
                    bloom.quality.value = (int)Data.BloomQuality.Value - 1;
                }
            }
            
            // GTAO Quality
            if (_volume.TryGet(out ScreenSpaceAmbientOcclusion ssao))
            {
                if (Data.AOQuality.Value == AmbientOcclusionQuality.Off)
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, false);
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, true);
                    ssao.quality.value = (int)Data.AOQuality.Value - 1;
                }
            }
            
            // Motion Blur Quality
            if (_volume.TryGet(out MotionBlur motionBlur))
            {
                if (Data.MotionBlur.Value == MotionBlurState.Off)
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, false);
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, true);
                    motionBlur.quality.value = (int)Data.MotionBlurQuality.Value;
                }
            }
            
            // SSR Quality
            if (_volume.TryGet(out ScreenSpaceReflection ssr))
            {
                if (Data.ScreenSpaceReflectionQuality.Value == ScreenSpaceReflectionQuality.Off)
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSR, false);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSRAsync, false);
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSR, true);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSRAsync, true);
                    ssr.quality.value = (int)Data.ScreenSpaceReflectionQuality.Value - 1;
                }
            }
            
            // Contact Shadows Quality
            if (_volume.TryGet(out ContactShadows contactShadows))
            {
                if (Data.ContactShadowsQuality.Value == ContactShadowsQuality.Off)
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadows, false);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadowsAsync, false);
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadows, true);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadowsAsync, true);
                    contactShadows.quality.value = (int)Data.ContactShadowsQuality.Value - 1;
                }
            }

            if (Data.SubSurfaceScatteringQuality.Value == SubSurfaceScatteringQuality.Off)
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
        public ReactiveVar<ShadowsQuality> SunShadowsQuality = new(ShadowsQuality.High);
        public ReactiveVar<ShadowsQuality> AdditionalShadowsQuality = new(ShadowsQuality.High);
        public ReactiveVar<BloomQuality> BloomQuality = new(Managers.BloomQuality.High);
        public ReactiveVar<AmbientOcclusionQuality> AOQuality = new(AmbientOcclusionQuality.Medium);
        public ReactiveVar<MotionBlurState> MotionBlur = new(MotionBlurState.On);
        public ReactiveVar<MotionBlurQuality> MotionBlurQuality = new(Managers.MotionBlurQuality.High);
        public ReactiveVar<TextureQuality> TextureQuality = new(Managers.TextureQuality.VeryHigh);
        public ReactiveVar<MeshQuality> MeshQuality = new(Managers.MeshQuality.VeryHigh);
        public ReactiveVar<ScreenSpaceReflectionQuality> ScreenSpaceReflectionQuality = new(Managers.ScreenSpaceReflectionQuality.Medium); 
        public ReactiveVar<ContactShadowsQuality> ContactShadowsQuality = new(Managers.ContactShadowsQuality.Medium);
        public ReactiveVar<SubSurfaceScatteringQuality> SubSurfaceScatteringQuality = new(Managers.SubSurfaceScatteringQuality.On);
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