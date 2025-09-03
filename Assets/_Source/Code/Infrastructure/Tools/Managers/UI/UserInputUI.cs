using SurgeEngine._Source.Code.UI.OptionBars;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers.UI
{
    public class UserInputUI : OptionUI
    {
        [SerializeField] private SliderOptionBar sensitivityBar;
        
        [Inject] private UserInput _userInput;
        
        protected override void Setup()
        {
            sensitivityBar.OnChanged += _ =>
            {
                _userInput.SetSensitivity(sensitivityBar.Slider.value / 100);
            };

            var data = _userInput.GetData();
            sensitivityBar.Slider.value = data.Sensitivity.Value * 100;
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