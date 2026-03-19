using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace SurgeEngine
{
    [CreateAssetMenu(fileName = "PlatformSprite", menuName = "Surge Engine/UI/PlatformSprite")]
    public class PlatformSprite : ScriptableObject
    {
        [SerializeField] private Sprite mkSprite;
        [SerializeField] private Sprite psSprite;
        [SerializeField] private Sprite xbSprite;

        public Sprite GetDeviceSprite()
        {
            switch (CharacterContext.Context.Input.GetDevice())
            {
                case GameDevice.Keyboard:
                    return mkSprite;
                case GameDevice.XboxController:
                    return xbSprite;
                case GameDevice.Playstation:
                    return xbSprite;
            }

            return null;
        }
    }
}
