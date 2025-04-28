using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Core.Actor.States
{
    public interface IDamageableState
    {
        void TakeDamage(ActorBase owner)
        {
            owner.stateMachine.SetState<FStateDamage>().SetState(owner.IsDead ? DamageState.Dead : DamageState.Alive);
        }
    }
}