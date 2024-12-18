using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Settings
{
    public class IndicatorElement : MonoBehaviour
    {
        [SerializeField] private Color simpleColor;
        [SerializeField] private Color activeColor;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetValue(bool value)
        {
            _image.color = value ? simpleColor : activeColor;
        }
    }
}