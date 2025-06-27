using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Core.Actor.States
{
    public interface IDamageableState
    {
        void TakeDamage(ActorBase owner)
        {
        }
    }
}