using SurgeEngine.Code.Actor.System;

namespace SurgeEngine.Code.Actor.States
{
    public interface IDamageableState
    {
        void TakeDamage(ActorBase owner, Entity sender)
        {
            owner.stateMachine.SetState<FStateDamage>();
        }
    }
}