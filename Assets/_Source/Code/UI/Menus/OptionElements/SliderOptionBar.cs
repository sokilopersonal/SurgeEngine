using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus.OptionElements
{
    public class SliderOptionBar : OptionBar
    {
        [Header("Slider")]
        [SerializeField] private Slider slider;
        [SerializeField, Tooltip("The step value used for slider movement when using left/right navigation")] 
        private float step = 10;
        [SerializeField] private string sliderTextFormat = "F2";
        
        public event Action<float> OnSliderBarValueChanged;

        protected override void Awake()
        {
            base.Awake();
            
            slider ??= GetComponentInChildren<Slider>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (slider != null)
            {
                slider.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (slider != null)
            {
                slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }
        }

        private void OnSliderValueChanged(float value)
        {
            Index = Mathf.RoundToInt(value);
            
            OnSliderBarValueChanged?.Invoke(value);
            
            stateText.text = value.ToString(sliderTextFormat, CultureInfo.InvariantCulture);
        }

        public override void SetIndex(int index)
        {
            base.SetIndex(index);
            
            slider.value = index;
            OnSliderValueChanged(index);
        }

        public void SetSliderValue(float value)
        {
            slider.value = value;
            OnSliderValueChanged(value);
        }

        public override void UpdateText(int index)
        {
            
        }

        protected override void AddIndexByMove(MoveDirection obj)
        {
            switch (obj)
            {
                case MoveDirection.Left:
                    slider.value -= step;
                    break;
                case MoveDirection.Right:
                    slider.value += step;
                    break;
            }
        }
    }
}