using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Source.Code.Input
{
    [CreateAssetMenu(fileName = "InputIconDatabase", menuName = "Input/Input Icon Database")]
    public class InputIconDatabase : ScriptableObject
    {
        public enum DeviceType
        {
            Keyboard,
            Mouse,
            Xbox,
            PlayStation,
            Any
        }

        [Serializable]
        public class Icon
        {
            public DeviceType device;
            public string controlPath;
            public Sprite sprite;
        }

        [SerializeField] private List<Icon> icons = new();

        public IReadOnlyList<Icon> Icons => icons;

        public Sprite GetSprite(string controlPath, DeviceType device)
        {
            if (string.IsNullOrEmpty(controlPath))
                return null;

            for (int i = 0; i < icons.Count; i++)
            {
                Icon icon = icons[i];

                if (icon.device != device)
                    continue;

                if (string.Equals(icon.controlPath, controlPath, StringComparison.OrdinalIgnoreCase))
                    return icon.sprite;
            }

            for (int i = 0; i < icons.Count; i++)
            {
                Icon icon = icons[i];

                if (icon.device != DeviceType.Any)
                    continue;

                if (string.Equals(icon.controlPath, controlPath, StringComparison.OrdinalIgnoreCase))
                    return icon.sprite;
            }

            return null;
        }

        public void SetSprite(string controlPath, DeviceType device, Sprite sprite)
        {
            for (int i = 0; i < icons.Count; i++)
            {
                Icon icon = icons[i];

                if (icon.device != device)
                    continue;

                if (!string.Equals(icon.controlPath, controlPath, StringComparison.OrdinalIgnoreCase))
                    continue;

                icon.sprite = sprite;
                return;
            }

            icons.Add(new Icon
            {
                device = device,
                controlPath = controlPath,
                sprite = sprite
            });
        }

        public void Remove(string controlPath, DeviceType device)
        {
            for (int i = icons.Count - 1; i >= 0; i--)
            {
                if (icons[i].device != device)
                    continue;

                if (!string.Equals(icons[i].controlPath, controlPath, StringComparison.OrdinalIgnoreCase))
                    continue;

                icons.RemoveAt(i);
            }
        }
    }
}