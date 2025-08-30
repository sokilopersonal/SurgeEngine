using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates
{
    public interface IBoostHandler
    {
        void BoostHandle(CharacterBase character, BoostConfig config);
    }
}