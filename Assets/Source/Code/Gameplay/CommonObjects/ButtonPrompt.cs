using Alchemy.Inspector;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine
{
    public class ButtonPrompt : StageObject
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

        [Header("Sound")]
        [SerializeField] private EventReference navigationSound;

        public float GetActiveTime() { return activeTime; }
        public Transform GetTransform() { return trackTransform; }
        public ButtonType GetButtonType() { return buttonType; }
        public Sprite GetSprite() {
            switch (buttonType)
            {
                case ButtonType.A:
                    return aPrompt.GetDeviceSprite();
                case ButtonType.B:
                    return bPrompt.GetDeviceSprite();
                case ButtonType.X:
                    return xPrompt.GetDeviceSprite();
                case ButtonType.Y:
                    return yPrompt.GetDeviceSprite();
                case ButtonType.LB:
                    return lbPrompt.GetDeviceSprite();
                case ButtonType.RB:
                    return rbPrompt.GetDeviceSprite();
                default:
                    return null;
            }
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            ObjectEvents.OnButtonPromptTriggered?.Invoke(this);
            RuntimeManager.PlayOneShot(navigationSound);
        }
    }
}
