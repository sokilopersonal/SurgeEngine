using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine._Source.Code.UI.OptionBars
{
    public class SliderOptionBar : OptionBar
    {
        [SerializeField] private Slider slider;
        [SerializeField] private string format = "{0}";
        public Slider Slider => slider;

        protected override void Awake()
        {
            base.Awake();
            
            slider.onValueChanged.AddListener(Set);
        }

        private void Set(float value)
        {
            base.Set(Mathf.RoundToInt(value));
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