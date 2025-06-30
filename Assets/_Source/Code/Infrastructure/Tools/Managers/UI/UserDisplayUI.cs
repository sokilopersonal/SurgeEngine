using System;
using SurgeEngine.Code.UI.OptionBars;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class UserDisplayUI : OptionUI
    {
        [SerializeField] private OptionBar antiAliasingQualityBar;
        [SerializeField] private SliderOptionBar sharpnessSliderBar;
        [SerializeField] private OptionBar fullscreenBar;
        
        [Inject] private UserDisplay _display;

        protected override void Start()
        {
            //var resolutions = InitializeResolutionOptions();

            var data = _display.GetData();
            
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
            
            fullscreenBar.OnChanged += b =>
            {
                _display.SetFullscreen(b.Index == 1);
                _display.Apply();
            };
            
            antiAliasingQualityBar.Set((int)data.antiAliasingQuality);
            sharpnessSliderBar.Slider.value = data.sharpness;
            fullscreenBar.Set(data.fullscreen ? 1 : 0);
            
            base.Start();
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
                antiAliasingQualityBar.Set((int)data.antiAliasingQuality);
                sharpnessSliderBar.Slider.value = data.sharpness * 100;
                /*int currentResolutionIndex = Array.FindIndex(InitializeResolutionOptions(), r => r.width == data.screenWidth && r.height == data.screenHeight);
                if (currentResolutionIndex >= 0)
                    resolutionDropdownBar.SetIndex(currentResolutionIndex);*/
                fullscreenBar.Set(data.fullscreen ? 1 : 0);
                
                _display.Apply();
                Save();
            });
        }
    }
}