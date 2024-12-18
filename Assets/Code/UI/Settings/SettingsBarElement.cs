using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Settings
{
    public class SettingsBarElement : Selectable, ISubmitHandler
    {
        [Header("Button")]
        [SerializeField] private UnityEvent Submit;
        
        [Header("Bar Element Name")]
        [SerializeField] private string barName;
        [SerializeField, ResizableTextArea] private string barDescription;
        [SerializeField] private TMP_Text barText;
        [SerializeField] private TMP_Text barIndicatorText;
        [SerializeField] private DescriptionElement barDescriptionElement;
        
        [Header("Bar Values")]
        [SerializeField] private string[] values = Array.Empty<string>();
        public Texture2D[] valueImagePreviews = Array.Empty<Texture2D>();
        [SerializeField] private IndicatorElement[] valueIndicators;
        private RawImage valueImage;

        public event Action<int> OnValueChanged; 
        private int _value;
        public int value
        {
            get => _value;
            set
            {
                if (value == _value) return;

                if (value >= values.Length || value < 0)
                {
                    _value = (int)Mathf.Repeat(value, values.Length);
                }
                else
                {
                    _value = value;
                }

                OnValueChanged?.Invoke(_value);
                UpdateValues();
            }
        }

        
        protected override void Start()
        {
            base.Start();
            
            if (Application.isPlaying) UpdateValues();
        }
        
        private void UpdateValues()
        {
            value = Mathf.Clamp(value, 0, values.Length - 1);
            
            UpdateName();

            if (values.Length > 0)
            {
                ToggleIndicatorView();
            }
        }
        
        private void UpdateName()
        {
            if (barText != null && barIndicatorText != null && barDescriptionElement != null)
            {
                barText.text = barName;
                barIndicatorText.text = values[value];
            }
        }

        private void ToggleIndicatorView()
        {
            for (int i = 0; i < valueIndicators.Length; i++)
            {
                valueIndicators[i].SetValue(i != value);
            }
        }

        public void AddValue()
        {
            value += 1;
        }
        
        public void SubtractValue()
        {
            value -= 1;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            DescriptionUpdate();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            
            DescriptionUpdate();
        }
        
        private void DescriptionUpdate()
        {
            barDescriptionElement.SetBarAndText(this, barDescription);
        }
        
        public void OnSubmit(BaseEventData eventData)
        {
            Submit.Invoke();
        }
    }
}