using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates
{
    public interface IBoostHandler
    {
        void BoostHandle(CharacterBase character, BoostConfig config);
    }
}