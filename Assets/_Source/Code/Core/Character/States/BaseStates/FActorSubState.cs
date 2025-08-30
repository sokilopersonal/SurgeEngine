using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.States.BaseStates
{
    public class FActorSubState : FSubState
    {
        protected CharacterBase character { get; private set; }

        public FActorSubState(CharacterBase owner)
        {
            character = owner;
        }
    }
}