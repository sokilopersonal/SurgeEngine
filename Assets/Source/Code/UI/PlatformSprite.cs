using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.UI
{
    [CreateAssetMenu(fileName = "PlatformSprite", menuName = "Surge Engine/UI/PlatformSprite")]
    public class PlatformSprite : ScriptableObject
    {
        [SerializeField] private Sprite mkSprite;
        [SerializeField] private Sprite psSprite;
        [SerializeField] private Sprite xbSprite;

        public Sprite GetDeviceSprite(GameDevice device)
        {
            switch (device)
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
