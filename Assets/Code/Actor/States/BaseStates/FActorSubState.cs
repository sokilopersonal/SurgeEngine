using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.Actor.States.BaseStates
{
    public class FActorSubState : FSubState
    {
        protected ActorBase actor { get; private set; }

        public FActorSubState(ActorBase owner)
        {
            actor = owner;
        }
    }
}