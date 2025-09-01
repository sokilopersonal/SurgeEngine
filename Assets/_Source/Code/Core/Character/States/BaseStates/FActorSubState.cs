using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;

namespace SurgeEngine._Source.Code.Core.Character.States.BaseStates
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