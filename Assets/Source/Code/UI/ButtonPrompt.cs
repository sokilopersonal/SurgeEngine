using DG.Tweening;
using SurgeEngine.Source.Code.UI.Animated;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

namespace SurgeEngine.Source.Code.UI
{
    public class ButtonPrompt : SelectReaction
    {
        [SerializeField] private bool selectToShow = true;
        [SerializeField] private Sprite mkSprite;
        [SerializeField] private Sprite psSprite;
        [SerializeField] private Sprite xbSprite;

        private Image _image;
        private Tween _tween;

        private void Awake()
        {
            _image = GetComponent<Image>();
            if (selectToShow) _image.DOFade(0, 0).SetUpdate(true);
        }

        private void Update()
        {
            SetDeviceSprite();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (selectToShow)
            {
                _tween?.Kill(true);
                _tween = _image.DOFade(1f, 0.1f).SetUpdate(true);
            }
        }
        
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            if (selectToShow)
            {
                _tween?.Kill(true);
                _tween = _image.DOFade(0f, 0.1f).SetUpdate(true);
            }
        }

        private void SetDeviceSprite()
        {
            var devices = InputSystem.devices;
            foreach (var device in devices)
            {
                if (device.wasUpdatedThisFrame)
                {
                    if (device is Keyboard)
                    {
                        _image.sprite = mkSprite;
                    }
                    else if (device is Mouse mouse)
                    {
                        if (mouse.delta.ReadValue().magnitude > 0.1f)
                        {
                            _image.sprite = mkSprite;
                        }
                    }
                    else if (device is DualShockGamepad)
                    {
                        _image.sprite = psSprite;
                    }
                    else if (device is XInputController)
                    {
                        _image.sprite = xbSprite;
                    }
                }
            }
        }
    }
}