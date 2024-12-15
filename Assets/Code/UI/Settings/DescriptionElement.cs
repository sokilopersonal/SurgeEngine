using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Settings
{
    public class DescriptionElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private RawImage previewImage;
        
        [SerializeField] private CanvasGroup group;
        private Tween _tween;
        private string _lastText;
        private SettingsBarElement _bar;

        private void Start()
        {
            previewImage.gameObject.SetActive(false);
        }

        public void SetBarAndText(SettingsBarElement bar, string text)
        {
            _bar = bar;
            UpdatePreview(_bar.value);
            _bar.OnValueChanged += UpdatePreview;

            if (bar != _bar)
            {
                _bar.OnValueChanged -= UpdatePreview;
            }
            
            if (_lastText != text)
            {
                _tween?.Kill();
                descriptionText.text = text;
                _lastText = text;
                _tween = group.DOFade(1f, 0.5f).From(0f).SetUpdate(true);
            }
        }

        private void UpdatePreview(int value)
        {
            var previews = _bar.valueImagePreviews;
            if (previews.Length > 0)
            {
                previewImage.gameObject.SetActive(true);;
                previewImage.texture = previews[Mathf.Clamp(value, 0, previews.Length - 1)];
            }
            else
            {
                previewImage.gameObject.SetActive(false);
            }
        }
    }
}