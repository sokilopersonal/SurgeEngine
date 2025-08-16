using System;
using System.Text.RegularExpressions;
using SurgeEngine.Code.UI.OptionBars;
using UnityEngine;
using UnityEngine.NVIDIA;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class UserDisplayUI : OptionUI
    {
        [SerializeField] private OptionBar fullscreenBar;
        [SerializeField] private OptionBar upscalingModeBar;
        [SerializeField] private OptionBar upscalingPresetBar;
        [SerializeField] private OptionBar antiAliasingQualityBar;
        [SerializeField] private SliderOptionBar sharpnessSliderBar;
        
        [Inject] private UserDisplay _display;

        protected override void Setup()
        {
            //var resolutions = InitializeResolutionOptions();
            
            upscalingModeBar.ClearOptions();
            foreach (UpscalingMode mode in Enum.GetValues(typeof(UpscalingMode)))
            {
                upscalingModeBar.AddOption(Enum.GetName(typeof(UpscalingMode), mode));
            }
            if (!IsDLSSCompatible)
            {
                upscalingModeBar.RemoveOption(Enum.GetName(typeof(UpscalingMode), UpscalingMode.DLSS));
            }
            
            upscalingPresetBar.ClearOptions();
            foreach (UpscalingQuality quality in Enum.GetValues(typeof(UpscalingQuality)))
            {
                var presetName = Regex.Replace(Enum.GetName(typeof(UpscalingQuality), quality) ?? string.Empty, "(?<!^)([A-Z])", " $1");
                upscalingPresetBar.AddOption(presetName);
            }

            var data = _display.GetData();

            fullscreenBar.OnChanged += b =>
            {
                _display.SetFullscreen(b.Index == 1);
                _display.Apply();
            };
            
            upscalingModeBar.OnChanged += b =>
            {
                UpscalingMode mode = Enum.Parse<UpscalingMode>(upscalingModeBar.CurrentValue);
                
                _display.SetUpscalingMode(mode);
                _display.Apply();
            };
            
            upscalingPresetBar.OnChanged += b =>
            {
                _display.SetUpscalingQuality((UpscalingQuality)b.Index);
                _display.Apply();
            };
            
            antiAliasingQualityBar.OnChanged += b =>
            {
                _display.SetAntiAliasing((AntiAliasingQuality)b.Index);
                _display.Apply();
            };

            sharpnessSliderBar.OnChanged += b =>
            {
                _display.SetSharpness(sharpnessSliderBar.Slider.value / 100f);
                _display.Apply();
            };

            fullscreenBar.Set(data.fullscreen ? 1 : 0);

            try
            {
                upscalingModeBar.Set((int)data.upscaleMode);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                
                if (data.upscaleMode == UpscalingMode.DLSS)
                {
                    if (!IsDLSSCompatible)
                    {
                        Debug.LogWarning("Chosen Upscaling method is DLSS, but no compatible device was found. Fallback to TAA.");
                        upscalingModeBar.Set(0);
                        Save();
                    }
                }
            }
            
            upscalingPresetBar.Set((int)data.upscaleQuality);
            antiAliasingQualityBar.Set((int)data.antiAliasingQuality);
            sharpnessSliderBar.Slider.value = data.sharpness * 100;
        }

        /*private Resolution[] InitializeResolutionOptions()
        {
            Resolution[] allResolutions = Screen.resolutions;
            int maxRefreshRate = (int)allResolutions.Max(r => r.refreshRateRatio.value);
            
            Resolution[] resolutions = allResolutions.Where(r => (int)r.refreshRateRatio.value == maxRefreshRate).ToArray();
            string[] resolutionStrings = new string[resolutions.Length];
            for (int i = 0; i < resolutions.Length; i++)
            {
                resolutionStrings[i] = $"{resolutions[i].width} x {resolutions[i].height}";
            }
            
            resolutionDropdownBar.Dropdown.ClearOptions();
            resolutionDropdownBar.Dropdown.AddOptions(new List<string>(resolutionStrings));
            return resolutions;
        }*/

        public override void Save()
        {
            _display.Save();
            
            base.Save();
        }

        public override void Revert()
        {
            _display.Load(data =>
            {
                fullscreenBar.Set(data.fullscreen ? 1 : 0);
                upscalingModeBar.Set((int)data.upscaleMode);
                upscalingPresetBar.Set((int)data.upscaleQuality);
                antiAliasingQualityBar.Set((int)data.antiAliasingQuality);
                sharpnessSliderBar.Slider.value = data.sharpness * 100;
                /*int currentResolutionIndex = Array.FindIndex(InitializeResolutionOptions(), r => r.width == data.screenWidth && r.height == data.screenHeight);
                if (currentResolutionIndex >= 0)
                    resolutionDropdownBar.SetIndex(currentResolutionIndex);*/
                
                _display.Apply();
                Save();
            });
        }
        
        private bool IsDLSSCompatible => SystemInfo.graphicsDeviceName.Contains("NVIDIA Geforce RTX", StringComparison.OrdinalIgnoreCase);
    }
}