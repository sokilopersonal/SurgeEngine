using System;
using SurgeEngine.Source.Code.Core.Character.Sound;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterSounds : CharacterComponent
    {
        private void Awake()
        {
            foreach (CharacterSound sound in GetComponents<CharacterSound>())
            {
                sound.Initialize(Character);
            }
        }
    }
}