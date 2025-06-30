using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.OptionBars
{
    public class SliderOptionBar : OptionBar
    {
        [SerializeField] private Slider slider;
        [SerializeField] private string format = "{0}";
        public Slider Slider => slider;

        protected override void Awake()
        {
            base.Awake();
            
            Index = (int)slider.value;
            
            slider.onValueChanged.AddListener(Set);
        }

        private void Set(float arg0)
        {
            Index = (int)arg0;
        }

        protected override void SetTextState()
        {
            state.text = string.Format("{0:" + format + "}", slider.value);
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);

            float value = 10f;
            if (eventData.moveDir == MoveDirection.Left)
                slider.value -= value;
            else if (eventData.moveDir == MoveDirection.Right)
                slider.value += value;
        }
    }
}