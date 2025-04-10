using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Zenject;
using Object = UnityEngine.Object;

namespace SurgeEngine.Code.Tools
{
    public class UserGraphics : ITickable
    {
        private const string Key = "GraphicsData";
        private readonly VolumeProfile _profile;

        private Light _sun;
        private HDAdditionalLightData _sunData;
        private readonly List<Light> _additionalLights;
        private readonly List<HDAdditionalLightData> _additionalLightsData;

        private readonly string[] _refractionQualityKeywords =
        {
            "_REFRACTIONQUALITY_NATIVE",
            "_REFRACTIONQUALITY_MEDIUM", 
            "_REFRACTIONQUALITY_LOW"
        };

        public UserGraphics(VolumeProfile profile)
        {
            _profile = profile;
            _additionalLights = new List<Light>();
            _additionalLightsData = new List<HDAdditionalLightData>();
            
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
            
            Debug.Log("Initialized UserGraphics");
        }

        public void SetTextureQuality(int value)
        {
            QualitySettings.globalTextureMipmapLimit = (int)Mathf.Lerp(3, 0, value / 3f); // Inverse value
        }

        public void SetSunShadowsQuality(int value)
        {
            var data = _sun.GetComponent<HDAdditionalLightData>();
            if (value == -1)
            {
                data.EnableShadows(false);
            }
            else
            {
                data.EnableShadows(true);
                data.shadowResolution.level = value;
            }
        }
        
        public void SetAdditionalShadowsQuality(int value)
        {
            foreach (var data in _additionalLights.Select(light => light.GetComponent<HDAdditionalLightData>()))
            {
                if (value == -1)
                {
                    data.EnableShadows(false);
                }
                else
                {
                    data.EnableShadows(true);
                    data.shadowResolution.level = value;
                }
            }
        }

        public void SetBloomQuality(int value)
        {
            if (_profile.TryGet(out Bloom bloom))
            {
                bloom.quality.value = value;
            }
        }
        
        public void SetAmbientOcclusionQuality(int value)
        {
            if (_profile.TryGet(out ScreenSpaceAmbientOcclusion ssao))
            {
                ssao.quality.value = value;
            }
        }

        public void SetMotionBlurQuality(int value)
        {
            if (_profile.TryGet(out MotionBlur motionBlur))
            {
                motionBlur.quality.value = value;
            }
        }

        public void SetRefractionQuality(RefractionQuality level)
        {
            SetKeyword(_refractionQualityKeywords, (int)level);
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
            // Test

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetTextureQuality(0);
                SetSunShadowsQuality(-1);
                SetAdditionalShadowsQuality(-1);
                SetRefractionQuality(RefractionQuality.Low);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetTextureQuality(1);
                SetSunShadowsQuality(0);
                SetAdditionalShadowsQuality(0);
                SetRefractionQuality(RefractionQuality.Medium);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetTextureQuality(2);
                SetSunShadowsQuality(1);
                SetAdditionalShadowsQuality(1);
                SetRefractionQuality(RefractionQuality.Native);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetTextureQuality(3);
                SetSunShadowsQuality(2);
                SetAdditionalShadowsQuality(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Save();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Load();
            }
        }

        public void Save()
        {
            var data = new GraphicsData
            {
                SunShadowsQuality = _sunData.GetComponent<Light>().shadows == LightShadows.Soft ? _sunData.shadowResolution.level : -1,
                AdditionalShadowsQuality = _additionalLightsData.All(light => light.GetComponent<Light>().shadows == LightShadows.Soft) ? _sunData.shadowResolution.level : -1,
                BloomQuality = _profile.TryGet(out Bloom bloom) ? bloom.quality.value : -1,
                AoQuality = _profile.TryGet(out ScreenSpaceAmbientOcclusion ssao) ? ssao.quality.value : -1,
                MotionBlurQuality = _profile.TryGet(out MotionBlur motionBlur) ? motionBlur.quality.value : -1,
                TextureQuality = (int)Mathf.Lerp(3, 0, QualitySettings.globalTextureMipmapLimit)
            };
            
            var json = JsonUtility.ToJson(data);
            Debug.Log(json);
            PlayerPrefs.SetString(Key, json);
        }
        
        public void Load()
        {
            if (PlayerPrefs.HasKey(Key))
            {
                var json = PlayerPrefs.GetString(Key);
                var data = JsonUtility.FromJson<GraphicsData>(json);
            
                SetSunShadowsQuality(data.SunShadowsQuality);
                SetAdditionalShadowsQuality(data.AdditionalShadowsQuality);
                SetBloomQuality(data.BloomQuality);
                SetAmbientOcclusionQuality(data.AoQuality);
                SetMotionBlurQuality(data.MotionBlurQuality);
                SetTextureQuality(data.TextureQuality);
            }
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
    }

    public class GraphicsData
    {
        public int SunShadowsQuality;
        public int AdditionalShadowsQuality;
        public int BloomQuality;
        public int AoQuality;
        public int MotionBlurQuality;
        public int TextureQuality;
    }
    
    public enum RefractionQuality
    {
        Native,
        Medium,
        Low
    }
}