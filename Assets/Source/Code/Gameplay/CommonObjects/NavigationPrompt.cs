using Alchemy.Inspector;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Source.Code.UI;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public class NavigationPrompt : StageObject
    {
        [FoldoutGroup("Button Sprites")]
        [SerializeField] private PlatformSprite aPrompt;
        [FoldoutGroup("Button Sprites")]
        [SerializeField] private PlatformSprite bPrompt;
        [FoldoutGroup("Button Sprites")]
        [SerializeField] private PlatformSprite xPrompt;
        [FoldoutGroup("Button Sprites")]
        [SerializeField] private PlatformSprite yPrompt;
        [FoldoutGroup("Button Sprites")]
        [SerializeField] private PlatformSprite lbPrompt;
        [FoldoutGroup("Button Sprites")]
        [SerializeField] private PlatformSprite rbPrompt;

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

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            ObjectEvents.OnButtonPromptTriggered?.Invoke(this);
            RuntimeManager.PlayOneShot(navigationSound);
        }

        public Sprite GetSprite()
        {
            var device = _character.Input.Device;
            
            switch (buttonType)
            {
                case ButtonType.A:
                    return aPrompt.GetDeviceSprite(device);
                case ButtonType.B:
                    return bPrompt.GetDeviceSprite(device);
                case ButtonType.X:
                    return xPrompt.GetDeviceSprite(device);
                case ButtonType.Y:
                    return yPrompt.GetDeviceSprite(device);
                case ButtonType.LB:
                    return lbPrompt.GetDeviceSprite(device);
                case ButtonType.RB:
                    return rbPrompt.GetDeviceSprite(device);
                default:
                    return null;
            }
        }
    }
}
