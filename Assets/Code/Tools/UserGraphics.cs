using System;
using System.Collections.Generic;
using System.IO;
using SurgeEngine.Code.CommonObjects.Lighting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.Tools
{
    public class UserGraphics : IInitializable, IStorageService, ILateDisposable
    {
        private const string FileName = "GraphicsSettings.json";
        private readonly VolumeProfile _volume;

        private readonly List<LightDefiner> _lightsData;

        private GraphicsData _data;
        private HDAdditionalCameraData _hdCameraData;
        
        private const int MaxTextureQuality = 3;
        private const int MaxRefractionQuality = 2;
        
        private readonly string[] _refractionQualityKeywords =
        {
            "_REFRACTIONQUALITY_NATIVE",
            "_REFRACTIONQUALITY_MEDIUM", 
            "_REFRACTIONQUALITY_LOW"
        };

        public UserGraphics(VolumeProfile profile)
        {
            _volume = profile;
            
            if (!_volume.TryGet(out ScreenSpaceReflection _))
            {
                _volume.Add<ScreenSpaceReflection>();
            }
            if (!_volume.TryGet(out Bloom _))
            {
                _volume.Add<Bloom>();
            }
            if (!_volume.TryGet(out ScreenSpaceAmbientOcclusion _))
            {
                _volume.Add<ScreenSpaceAmbientOcclusion>();
            }
            
            _lightsData = new List<LightDefiner>();
            Load<GraphicsData>(data => _data = data);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        public void Initialize()
        {
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
            _data.textureQuality = value;
        }

        public void SetSunShadowsQuality(ShadowsQuality value)
        {
            _data.sunShadowsQuality = value;
        }

        public void SetAdditionalShadowsQuality(ShadowsQuality value)
        {
            _data.additionalShadowsQuality = value;
        }

        public void SetBloomQuality(BloomQuality value)
        {
            _data.bloomQuality = value;
        }

        public void SetAmbientOcclusionQuality(AmbientOcclusionQuality value)
        {
            _data.aoQuality = value;
        }

        public void SetMotionBlurQuality(MotionBlurQuality value)
        {
            _data.motionBlurQuality = value;
        }

        public void SetRefractionQuality(RefractionQuality level)
        {
            _data.refractionQuality = level;
        }

        public void SetScreenSpaceReflectionsQuality(ScreenSpaceReflectionQuality level)
        {
            _data.screenSpaceReflectionQuality = level;
        }

        public void SetAntiAliasing(AntiAliasingQuality level)
        {
            _data.antiAliasingQuality = level;
        }

        public void SetContactShadows(ContactShadowsQuality level)
        {
            _data.contactShadowsQuality = level;
        }

        public void SetSubSurfaceScattering(SubSurfaceScatteringQuality level)
        {
            _data.subSurfaceScatteringQuality = level;
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
            if (Camera.main != null)
            {
                _hdCameraData = Camera.main.GetComponent<HDAdditionalCameraData>();
                _hdCameraData.customRenderingSettings = true;
            }
            
            // Texture quality
            QualitySettings.globalTextureMipmapLimit = MaxTextureQuality - (int)_data.textureQuality;
            
            // Sun Shadows
            foreach (var light in _lightsData)
            {
                if (light.Component.type == LightType.Directional)
                {
                    var data = light.Data;
                    data.shadowResolution.level = (int)_data.sunShadowsQuality;
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
                data.shadowResolution.level = (int)_data.additionalShadowsQuality;
            }
            
            // Bloom Quality
            if (_volume.TryGet(out Bloom bloom))
            {
                if (_data.bloomQuality == BloomQuality.Off)
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Bloom, false);
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Bloom, true);
                    bloom.quality.value = (int)_data.bloomQuality - 1;
                }
            }
            
            // GTAO Quality
            if (_volume.TryGet(out ScreenSpaceAmbientOcclusion ssao))
            {
                if (_data.aoQuality == AmbientOcclusionQuality.Off)
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, false);
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSAO, true);
                    ssao.quality.value = (int)_data.aoQuality - 1;
                }
            }
            
            // Motion Blur Quality
            if (_volume.TryGet(out MotionBlur motionBlur))
            {
                if (_data.motionBlurQuality == MotionBlurQuality.Off)
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, false);
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.MotionBlur, true);
                    motionBlur.quality.value = (int)_data.motionBlurQuality - 1;
                }
            }
            
            // SSR Quality
            if (_volume.TryGet(out ScreenSpaceReflection ssr))
            {
                if (_data.screenSpaceReflectionQuality == ScreenSpaceReflectionQuality.Off)
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSR, false);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSRAsync, false);
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSR, true);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSRAsync, true);
                    ssr.quality.value = (int)_data.screenSpaceReflectionQuality - 1;
                }
            }
            
            // Contact Shadows Quality
            if (_volume.TryGet(out ContactShadows contactShadows))
            {
                if (_data.contactShadowsQuality == ContactShadowsQuality.Off)
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadows, false);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadowsAsync, false);
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadows, true);
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.ContactShadowsAsync, true);
                    contactShadows.quality.value = (int)_data.contactShadowsQuality - 1;
                }
            }

            if (_data.subSurfaceScatteringQuality == SubSurfaceScatteringQuality.Off)
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
            SetKeyword(_refractionQualityKeywords, MaxRefractionQuality - (int)_data.refractionQuality);
            
            if (_hdCameraData)
            {
                _hdCameraData.TAAQuality = (HDAdditionalCameraData.TAAQualityLevel)_data.antiAliasingQuality;
            }
            else
            {
                Debug.LogWarning("[UserGraphics] HDCameraData doesn't exists.");
            }

            Debug.Log($"[UserGraphics] Applied graphics options");
        }

        public void Save(Action<bool> callback = null)
        {
            if (_data != null)
            {
                var path = GetDataPath();
                _data.textureQuality = (TextureQuality)(MaxTextureQuality - QualitySettings.globalTextureMipmapLimit);

                if (_lightsData.Count > 0)
                {
                    var sun = _lightsData.Find(x => x.Component.type == LightType.Directional);
                    if (sun)
                    {
                        _data.sunShadowsQuality = (ShadowsQuality)sun.Data.shadowResolution.level;
                    }
                    
                    var additionalLight = _lightsData.Find(x => x.Component.type != LightType.Directional);
                    if (additionalLight)
                    {
                        _data.additionalShadowsQuality = (ShadowsQuality)sun.Data.shadowResolution.level;
                    }
                }
                
                _data.bloomQuality = _volume.TryGet(out Bloom bloom) ? _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.Bloom) ? (BloomQuality)bloom.quality.value + 1 : BloomQuality.Off : BloomQuality.Medium;
                _data.aoQuality = _volume.TryGet(out ScreenSpaceAmbientOcclusion ssao) ? _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.SSAO) ? (AmbientOcclusionQuality)ssao.quality.value + 1 : AmbientOcclusionQuality.Off : AmbientOcclusionQuality.Medium;
                _data.motionBlurQuality = _volume.TryGet(out MotionBlur motionBlur) ? _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.MotionBlur) ? (MotionBlurQuality)motionBlur.quality.value + 1 : MotionBlurQuality.Off : MotionBlurQuality.Medium;
                _data.refractionQuality = (RefractionQuality)MaxRefractionQuality - Array.FindIndex(_refractionQualityKeywords, Shader.IsKeywordEnabled);
                _data.screenSpaceReflectionQuality = _volume.TryGet(out ScreenSpaceReflection ssr) ? _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.SSR) ? (ScreenSpaceReflectionQuality)ssr.quality.value + 1 : ScreenSpaceReflectionQuality.Off : ScreenSpaceReflectionQuality.High;
                _data.antiAliasingQuality = (AntiAliasingQuality)_hdCameraData.TAAQuality;
                _data.contactShadowsQuality = _volume.TryGet(out ContactShadows cs) ? _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.ContactShadows) ? (ContactShadowsQuality)cs.quality.value + 1 : ContactShadowsQuality.Off : ContactShadowsQuality.Medium;
                _data.subSurfaceScatteringQuality = _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.Transmission) 
                                                    && _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.SubsurfaceScattering) ? SubSurfaceScatteringQuality.On : SubSurfaceScatteringQuality.Off;
                
                File.WriteAllText(path, JsonUtility.ToJson(_data, true));
            
                callback?.Invoke(true);
            }
            else
            {
                _data = new GraphicsData();
                Save(callback);
            }
        }

        public void Load<T>(Action<T> callback = null)
        {
            if (File.Exists(GetDataPath()))
            {
                var data = JsonUtility.FromJson<T>(File.ReadAllText(GetDataPath()));
                callback?.Invoke(data);
            }
        }

        public GraphicsData GetGraphicsData() => _data;

        public string GetDataPath() => Path.Combine(Application.persistentDataPath, FileName);
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
        public AntiAliasingQuality antiAliasingQuality = AntiAliasingQuality.High;
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

    public enum AntiAliasingQuality
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