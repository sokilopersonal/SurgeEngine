namespace SurgeEngine.Code.Core.Actor.System
{
    public class CharacterContext
    {
        private static CharacterBase _character;
        public static CharacterBase Context => _character;
        
        public static void Set(CharacterBase character)
        {
            _character = character;
        }
    }
}