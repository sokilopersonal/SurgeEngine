using SurgeEngine.Source.Code.UI.OptionBars;
using System;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Infrastructure.Tools.Managers.UI
{
    public class UserInputUI : OptionUI
    {
        [SerializeField] private SliderOptionBar sensitivityBar;
        [SerializeField] private NavigationOptionBar homingBar;
        
        [Inject] private UserInput _userInput;
        
        protected override void Setup()
        {
            homingBar.OnChanged += _ =>
            {
                _userInput.SetHoming(homingBar.CurrentValue);
            };

            sensitivityBar.OnChanged += _ =>
            {
                _userInput.SetSensitivity(sensitivityBar.Slider.value / 100);
            };

            var data = _userInput.GetData();
            sensitivityBar.Slider.value = data.Sensitivity.Value * 100;
            homingBar.Set(Convert.ToInt32(data.homingOnX.Value));
        }

        public override void Save()
        {
            base.Save();
            
            _userInput.Save();
        }

        public override void Revert()
        {
            base.Revert();
            
            _userInput.Load(data =>
            {
                sensitivityBar.Slider.value = data.Sensitivity.Value * 100;
                Save();
            });
        }
    }
}