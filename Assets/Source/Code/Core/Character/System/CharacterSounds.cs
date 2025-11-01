using SurgeEngine.Source.Code.Core.Character.Sound;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterSounds : CharacterComponent
    {
        internal override void Set(CharacterBase character)
        {
            base.Set(character);
            
            foreach (CharacterSound sound in GetComponents<CharacterSound>())
            {
                sound.Initialize(character);
            }
        }
    }
}