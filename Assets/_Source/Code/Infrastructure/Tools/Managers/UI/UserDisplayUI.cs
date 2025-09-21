using System;
using System.Text.RegularExpressions;
using SurgeEngine._Source.Code.UI.OptionBars;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers.UI
{
    public class UserDisplayUI : OptionUI
    {
        [SerializeField] private OptionBar fullscreenBar;
        [SerializeField] private OptionBar upscalingModeBar;
        [SerializeField] private OptionBar upscalingPresetBar;
        [SerializeField] private OptionBar antiAliasingBar;
        [SerializeField] private OptionBar antiAliasingQualityBar;
        [SerializeField] private SliderOptionBar sharpnessSliderBar;
        
        [Inject] private UserDisplay _display;

        protected override void Setup()
        {
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
            };
            
            upscalingModeBar.OnChanged += b =>
            {
                UpscalingMode mode = Enum.Parse<UpscalingMode>(upscalingModeBar.CurrentValue);
                
                _display.SetUpscalingMode(mode);
                
                upscalingPresetBar.gameObject.SetActive(mode != UpscalingMode.Off);
            };
            
            upscalingPresetBar.OnChanged += b =>
            {
                _display.SetUpscalingQuality((UpscalingQuality)b.Index);
            };

            antiAliasingBar.OnChanged += b =>
            {
                AntiAliasing mode = Enum.Parse<AntiAliasing>(antiAliasingBar.CurrentValue);

                _display.SetAntiAliasing(mode);
                
                antiAliasingQualityBar.gameObject.SetActive(mode is AntiAliasing.SMAA or AntiAliasing.TAA);
                sharpnessSliderBar.gameObject.SetActive(mode == AntiAliasing.TAA);
            };
            
            antiAliasingQualityBar.OnChanged += b =>
            {
                _display.SetAntiAliasingQuality((AntiAliasingQuality)b.Index);
            };

            sharpnessSliderBar.OnChanged += b =>
            {
                _display.SetSharpness(sharpnessSliderBar.Slider.value / 100f);
            };

            fullscreenBar.Set(data.Fullscreen.Value ? 1 : 0);
            upscalingModeBar.Set((int)data.UpscaleMode.Value);
            upscalingPresetBar.Set((int)data.UpscaleQuality.Value);
            antiAliasingBar.Set((int)data.AntiAliasing.Value);
            antiAliasingQualityBar.Set((int)data.AntiAliasingQuality.Value);
            sharpnessSliderBar.Slider.value = data.Sharpness.Value * 100;
        }

        public override void Save()
        {
            _display.Save();
            
            base.Save();
        }

        public override void Revert()
        {
            base.Revert();
            
            _display.Load(data =>
            {
                fullscreenBar.Set(data.Fullscreen.Value ? 1 : 0);
                upscalingModeBar.Set((int)data.UpscaleMode.Value);
                upscalingPresetBar.Set((int)data.UpscaleQuality.Value);
                antiAliasingBar.Set((int)data.AntiAliasing.Value);
                antiAliasingQualityBar.Set((int)data.AntiAliasingQuality.Value);
                sharpnessSliderBar.Slider.value = data.Sharpness.Value * 100;
                
                Save();
            });
        }
        
        private bool IsDLSSCompatible => HDDynamicResolutionPlatformCapabilities.DLSSDetected;
    }
}