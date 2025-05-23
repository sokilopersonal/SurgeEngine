using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class UserDisplayUI : OptionUI
    {
        [SerializeField] private OptionBar antiAliasingQualityBar;
        [SerializeField] private SliderOptionBar sharpnessSliderBar;
        
        [Inject] private UserDisplay _display;

        protected override void Awake()
        {
            base.Awake();
            
            antiAliasingQualityBar.OnIndexChanged += i => _display.SetAntiAliasing((AntiAliasingQuality)i);
            sharpnessSliderBar.OnSliderBarValueChanged += value => _display.SetSharpness(value);

            var data = _display.GetData();
            antiAliasingQualityBar.SetIndex((int)data.antiAliasingQuality);
            sharpnessSliderBar.SetSliderValue(data.sharpness);
            
            antiAliasingQualityBar.OnIndexChanged += _ => _display.Apply();
            sharpnessSliderBar.OnSliderBarValueChanged += _ => _display.Apply();
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
                
                _display.Apply();
                Save();
            });
        }
    }
}