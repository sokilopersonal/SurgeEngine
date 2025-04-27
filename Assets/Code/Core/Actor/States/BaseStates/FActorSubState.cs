using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.States.BaseStates
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