using SurgeEngine.Code.Config.Graphics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace SurgeEngine.Code.Tools
{
    public class UserGraphics : MonoBehaviour
    {
        [SerializeField] private Light _directionalLight;
        [SerializeField] private Volume _globalVolumeProfile;
        
        private HDRenderPipelineAsset _pipelineAsset => (HDRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        private HDAdditionalCameraData _cameraData;
        
        private FrameSettings _frameSettings;

        private static UserGraphics _instance;
        public static UserGraphics Instance => _instance;

        public QualityData data;
        
        private const string UserGraphicsKey = "UserGraphics";

        private void Awake()
        {
            _instance = this;
            
            _cameraData = Camera.main.GetComponent<HDAdditionalCameraData>();
            _cameraData.customRenderingSettings = true;

            _frameSettings = _cameraData.renderingPathCustomFrameSettings;
            
            Load();
        }

        private void Update()
        {
            _cameraData.renderingPathCustomFrameSettings = _frameSettings;
        }

        public void SetDirectionalLightQuality(int resolution)
        {
            var lightData = _directionalLight.GetComponent<HDAdditionalLightData>();
            lightData.SetShadowResolutionOverride(false);
            lightData.SetShadowResolutionLevel(resolution);
            
            data.shadowsQuality = resolution;
            Save();
        }

        public void SetPCSS(bool value)
        {
            var pcssData = _pipelineAsset.currentPlatformRenderPipelineSettings;
            pcssData.hdShadowInitParams.directionalShadowFilteringQuality = value ? HDShadowFilteringQuality.High : HDShadowFilteringQuality.Medium;
            
            _pipelineAsset.currentPlatformRenderPipelineSettings = pcssData;
            
            data.pcssShadows = value;
            Save();
        }
        
        public void SetBloom(bool bloomBarValue)
        {
            _frameSettings.SetEnabled(FrameSettingsField.Bloom, bloomBarValue);
            
            data.bloom = bloomBarValue;
            
            Save();
        }
        
        public void SetMotionBlur(bool value)
        {
            _frameSettings.SetEnabled(FrameSettingsField.MotionBlur, value);
            
            data.motionBlur = value;
            
            Save();
        }
        
        public void SetMotionBlurQuality(int value)
        {
            _globalVolumeProfile.profile.TryGet(out MotionBlur motionBlur);
            motionBlur.quality.value = Mathf.Min(value, 2);
            
            data.motionBlurQuality = value;
            
            Save();
        }
        
        public void SetSSR(bool value)
        {
            _frameSettings.SetEnabled(FrameSettingsField.SSR, value);
            
            data.ssr = value;
            
            Save();
        }
        
        public void SetSSRQuality(int value)
        {
            _globalVolumeProfile.profile.TryGet(out ScreenSpaceReflection ssr);
            ssr.quality.value = Mathf.Min(value, 2);
            
            data.ssrQuality = value;
            
            Save();
        }
        
        public void SetAA(int value)
        {
            _cameraData.antialiasing = (HDAdditionalCameraData.AntialiasingMode)value;
            _frameSettings.SetEnabled(FrameSettingsField.Antialiasing, value != 0);
            
            data.antiAliasing = value;
            
            Save();
        }
        
        public void Save()
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(UserGraphicsKey, json);
        }
        
        public void Load()
        {
            if (PlayerPrefs.HasKey(UserGraphicsKey))
            {
                var json = PlayerPrefs.GetString(UserGraphicsKey);
                data = JsonUtility.FromJson<QualityData>(json);
                
                SetDirectionalLightQuality(data.shadowsQuality);
                SetPCSS(data.pcssShadows);
                SetBloom(data.bloom);
                SetMotionBlur(data.motionBlur);
                SetMotionBlurQuality(data.motionBlurQuality);
                SetSSR(data.ssr);
                SetSSRQuality(data.ssrQuality);
            }
        }
    }
}