using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateReactionPlate : FCharacterState
    {
        public FStateReactionPlate(CharacterBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.ResetVelocity();
        }
    }
}