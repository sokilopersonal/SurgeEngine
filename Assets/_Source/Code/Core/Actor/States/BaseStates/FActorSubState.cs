using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.States.BaseStates
{
    public class FActorSubState : FSubState
    {
        protected ActorBase Actor { get; private set; }

        public FActorSubState(ActorBase owner)
        {
            Actor = owner;
        }
    }
}