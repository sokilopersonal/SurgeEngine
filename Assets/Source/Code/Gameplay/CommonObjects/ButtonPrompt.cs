using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using UnityEngine;

namespace SurgeEngine
{
    public class ButtonPrompt : StageObject
    {
        [System.Serializable]
        private struct PlatformSprite
        {
            public Sprite keyboardSprite;
            public Sprite xboxSprite;
            public Sprite playstationSprite;
        }

        [Header("General")]
        [SerializeField] private PlatformSprite sprite;
        [Space(10)]
        [SerializeField] private float activeTime;
        [SerializeField] private Transform trackTransform;

        [Header("Sound")]
        [SerializeField] private EventReference navigationSound;

        public float GetActiveTime() { return activeTime; }
        public Transform GetTransform() { return trackTransform; }
        public Sprite GetSprite() {
            switch (CharacterContext.Context.Input.GetDevice())
            {
                case GameDevice.Keyboard:
                    return sprite.keyboardSprite;
                case GameDevice.XboxController:
                    return sprite.xboxSprite;
                case GameDevice.Playstation:
                    return sprite.playstationSprite;
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
