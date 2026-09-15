using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public class NavigationPrompt : StageObject
    {
        [Header("General")]
        [SerializeField] private ButtonType buttonType;
        [SerializeField] private float activeTime;
        [SerializeField] private Transform trackTransform;
        public ButtonType ButtonType => buttonType;
        public float ActiveTime => activeTime;
        public Transform TrackTransform => trackTransform;

        [Header("Sound")]
        [SerializeField] private EventReference navigationSound;

        [Inject] private CharacterBase _character;
        [Inject] private InputIconResolver _inputIconResolver;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            ObjectEvents.OnButtonPromptTriggered?.Invoke(this);
            RuntimeManager.PlayOneShot(navigationSound);
        }

        public Sprite GetSprite()
        {
            InputBinding binding = _character.Input.GetInputBinding(buttonType);

            return _inputIconResolver.GetSprite(
                binding,
                GetIconDeviceType());
        }

        private InputIconDatabase.DeviceType GetIconDeviceType()
        {
            return _character.Input.Device switch
            {
                GameDevice.Keyboard => InputIconDatabase.DeviceType.Keyboard,
                GameDevice.XboxController => InputIconDatabase.DeviceType.Xbox,
                GameDevice.Playstation => InputIconDatabase.DeviceType.PlayStation,
                _ => InputIconDatabase.DeviceType.Any
            };
        }
    }
}
