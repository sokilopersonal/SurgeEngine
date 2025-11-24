using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;

namespace SurgeEngine.Source.Code.Core.Character.States.BaseStates
{
    public class FCharacterSubState : FSubState
    {
        protected CharacterBase Character { get; private set; }

        public FCharacterSubState(CharacterBase owner)
        {
            Character = owner;
        }
    }
}