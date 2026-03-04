using System.Collections.Generic;
using Alchemy.Inspector;
using SurgeEngine.Source.Code.Core.Character.Sound;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterSounds : CharacterComponent
    {
        [SerializeField, ReadOnly] private List<CharacterSound> sounds = new();
        
        private void Awake()
        {
            foreach (CharacterSound sound in GetComponents<CharacterSound>())
            {
                sound.Initialize(Character);
                sounds.Add(sound);
            }
        }

        public T Get<T>()
        {
            for (var i = 0; i < sounds.Count; i++)
            {
                var sound = sounds[i];
                if (sound is T result)
                {
                    return result;
                }
            }

            return default;
        }
    }
}