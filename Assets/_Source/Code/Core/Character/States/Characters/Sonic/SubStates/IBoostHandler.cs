using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Config.SonicSpecific;

namespace SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates
{
    public interface IBoostHandler
    {
        void BoostHandle(CharacterBase character, BoostConfig config);
    }
}