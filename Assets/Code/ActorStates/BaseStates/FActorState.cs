using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FActorState : FState
    {
        protected Actor actor { get; private set; }

        public void SetOwner(Actor owner)
        {
            actor = owner;
        }
    }
}