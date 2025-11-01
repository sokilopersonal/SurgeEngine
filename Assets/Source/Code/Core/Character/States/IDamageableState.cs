using SurgeEngine.Source.Code.Core.Character.System;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public interface IDamageableState
    {
        void TakeDamage(CharacterBase owner)
        {
        }
    }
}