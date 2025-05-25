using System;
using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class UserDisplayUI : OptionUI
    {
        [SerializeField] private OptionBar antiAliasingQualityBar;
        [SerializeField] private SliderOptionBar sharpnessSliderBar;
        [SerializeField] private DropdownOptionBar resolutionDropdownBar;
        [SerializeField] private OptionBar fullscreenBar;
        
        [Inject] private UserDisplay _display;

        protected override void Awake()
        {
            base.Awake();
            
            var resolutions = InitializeResolutionOptions();

            antiAliasingQualityBar.OnIndexChanged += i => _display.SetAntiAliasing((AntiAliasingQuality)i);
            sharpnessSliderBar.OnSliderBarValueChanged += value => _display.SetSharpness(value);
            resolutionDropdownBar.OnDropdownBarValueChanged += i => _display.SetResolution(new Vector2(resolutions[i].width, resolutions[i].height));
            fullscreenBar.OnIndexChanged += i => _display.SetFullscreen(i == 1);

            var data = _display.GetData();
            antiAliasingQualityBar.SetIndex((int)data.antiAliasingQuality);
            sharpnessSliderBar.SetSliderValue(data.sharpness);
            int currentResolutionIndex = Array.FindIndex(resolutions, r => r.width == data.screenWidth && r.height == data.screenHeight);
            if (currentResolutionIndex >= 0)
                resolutionDropdownBar.SetIndex(currentResolutionIndex);
            fullscreenBar.SetIndex(data.fullscreen ? 1 : 0);
            
            antiAliasingQualityBar.OnIndexChanged += _ => _display.Apply();
            sharpnessSliderBar.OnSliderBarValueChanged += _ => _display.Apply();
            resolutionDropdownBar.OnDropdownBarValueChanged += _ => _display.Apply();
            fullscreenBar.OnIndexChanged += _ => _display.Apply();
        }

        private Resolution[] InitializeResolutionOptions()
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
        }

        public override void Save()
        {
            _display.Save();
            
            base.Save();
        }

        public override void Revert()
        {
            _display.Load(data =>
            {
                antiAliasingQualityBar.SetIndex((int)data.antiAliasingQuality);
                sharpnessSliderBar.SetSliderValue(data.sharpness);
                int currentResolutionIndex = Array.FindIndex(InitializeResolutionOptions(), r => r.width == data.screenWidth && r.height == data.screenHeight);
                if (currentResolutionIndex >= 0)
                    resolutionDropdownBar.SetIndex(currentResolutionIndex);
                fullscreenBar.SetIndex(data.fullscreen ? 1 : 0);
                
                _display.Apply();
                Save();
            });
        }
    }
}