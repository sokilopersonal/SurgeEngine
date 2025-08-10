using SurgeEngine.Code.Core.Actor.Sound;

namespace SurgeEngine.Code.Core.Actor.System
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