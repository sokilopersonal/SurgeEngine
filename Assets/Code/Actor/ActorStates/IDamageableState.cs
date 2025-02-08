using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public interface IDamageableState
    {
        void TakeDamage(Actor owner, Entity sender)
        {
            owner.stateMachine.SetState<FStateDamage>();
        }
    }
}