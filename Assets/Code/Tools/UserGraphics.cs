using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Zenject;
using Object = UnityEngine.Object;

namespace SurgeEngine.Code.Tools
{
    public class UserGraphics : IInitializable, ITickable
    {
        private const string FileName = "GraphicsSettings.json";
        private readonly VolumeProfile _volume;

        private Light _sun;
        private HDAdditionalLightData _sunData;
        private readonly List<Light> _additionalLights;
        private readonly List<HDAdditionalLightData> _additionalLightsData;

        private readonly GraphicsData _data;
        private HDAdditionalCameraData _hdCameraData;
        
        private const int MaxTextureQuality = 3;
        private const int MaxRefractionQuality = 2;

        public event Action<GraphicsData> OnDataLoaded;
        public event Action<GraphicsData> OnDataApplied;

        private readonly string[] _refractionQualityKeywords =
        {
            "_REFRACTIONQUALITY_NATIVE",
            "_REFRACTIONQUALITY_MEDIUM", 
            "_REFRACTIONQUALITY_LOW"
        };
        
        private readonly string[] _materialQualityKeywords =
        {
            "_MATERIAL_QUALITY_HIGH",
            "_MATERIAL_QUALITY_MEDIUM", 
            "_MATERIAL_QUALITY_LOW"
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
            
            _additionalLights = new List<Light>();
            _additionalLightsData = new List<HDAdditionalLightData>();

            _data = Load();
        }
        
        public void Initialize()
        {
            FindLights();
            Apply();
        }

        private void FindLights()
        {
            _additionalLights.Clear();
            _additionalLightsData.Clear();
            
            var lights = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    SetSun(light);
                }
                else
                { 
                    AddAdditionalLight(light);
                }
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

        private void SetSun(Light sun)
        {
            if (sun.type == LightType.Directional)
            {
                _sun = sun;
                _sunData = sun.GetComponent<HDAdditionalLightData>();
            }
            else
            {
                Debug.LogError("This is not a directional light (sun).");
            }
        }

        private void AddAdditionalLight(Light light)
        {
            if (light.type != LightType.Directional)
            {
                _additionalLights.Add(light);
                _additionalLightsData.Add(light.GetComponent<HDAdditionalLightData>());
            }
            else
            {
                Debug.LogError("This is not a additional light.");
            }
        }

        public void Tick()
        {
            
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
            if (_sun != null) 
            {
                var data = _sun.GetComponent<HDAdditionalLightData>();
                data.EnableShadows(true);
                data.shadowResolution.level = (int)_data.sunShadowsQuality;
            }
            
            // Additional Shadows
            foreach (var data in _additionalLights)
            {
                //var lightData = data.GetComponent<HDAdditionalLightData>();
                //lightData.shadowResolution.level = (int)_data.additionalShadowsQuality;
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
                }
                else
                {
                    _hdCameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.SSR, true);
                    ssr.quality.value = (int)_data.screenSpaceReflectionQuality - 1;
                }
            }
            
            // Contact Shadows Quality
            if (_volume.TryGet(out ContactShadows contactShadows))
            {
                contactShadows.enable.overrideState = true;
                if (_data.contactShadowsQuality == ContactShadowsQuality.Off)
                {
                    contactShadows.enable.value = false;
                }
                else
                {
                    contactShadows.enable.value = true;
                    contactShadows.quality.value = (int)_data.contactShadowsQuality - 1;
                }
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
        }

        public void Save()
        {
            if (_data != null)
            {
                var path = GetDataPath();
                _data.textureQuality = (TextureQuality)(MaxTextureQuality - QualitySettings.globalTextureMipmapLimit);
                _data.sunShadowsQuality = (ShadowsQuality)_sunData.shadowResolution.level;
                if (_additionalLightsData.Count > 0) _data.additionalShadowsQuality = (ShadowsQuality)_additionalLightsData[0].shadowResolution.level;
                _data.bloomQuality = _volume.TryGet(out Bloom bloom) ? _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.Bloom) ? (BloomQuality)bloom.quality.value + 1 : BloomQuality.Off : BloomQuality.Medium;
                _data.aoQuality = _volume.TryGet(out ScreenSpaceAmbientOcclusion ssao) ? _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.SSAO) ? (AmbientOcclusionQuality)ssao.quality.value + 1 : AmbientOcclusionQuality.Off : AmbientOcclusionQuality.Medium;
                _data.motionBlurQuality = _volume.TryGet(out MotionBlur motionBlur) ? _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.MotionBlur) ? (MotionBlurQuality)motionBlur.quality.value + 1 : MotionBlurQuality.Off : MotionBlurQuality.Medium;
                _data.refractionQuality = (RefractionQuality)MaxRefractionQuality - Array.FindIndex(_refractionQualityKeywords, Shader.IsKeywordEnabled);
                _data.screenSpaceReflectionQuality = _volume.TryGet(out ScreenSpaceReflection ssr) ? _hdCameraData.renderingPathCustomFrameSettings.IsEnabled(FrameSettingsField.SSR) ? (ScreenSpaceReflectionQuality)ssr.quality.value + 1 : ScreenSpaceReflectionQuality.Off : ScreenSpaceReflectionQuality.High;
                _data.antiAliasingQuality = (AntiAliasingQuality)_hdCameraData.TAAQuality;
                _data.contactShadowsQuality = _volume.TryGet(out ContactShadows cs) ? cs.enable.value ? (ContactShadowsQuality)cs.quality.value + 1 : ContactShadowsQuality.Off : ContactShadowsQuality.Medium;
                
                OnDataApplied?.Invoke(_data);
            
                File.WriteAllText(path, JsonUtility.ToJson(_data, true));
            }
        }
        
        public GraphicsData Load()
        {
            if (File.Exists(GetDataPath()))
            {
                var data = JsonUtility.FromJson<GraphicsData>(File.ReadAllText(GetDataPath()));
                OnDataLoaded?.Invoke(data);
                return data;
            }

            return new GraphicsData();
        }
        
        public GraphicsData GetGraphicsData() => _data;

        private string GetDataPath() => Application.persistentDataPath + "/" + FileName;
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
}