using SurgeEngine.Source.Code.Core.Character.System;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateGrindSquat : FStateGrind
    {
        public FStateGrindSquat(CharacterBase owner) : base(owner)
        {
            gravityPower = 14;
        }

        public override void OnEnter()
        {
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Input.BHeld)
            {
                StateMachine.SetState<FStateGrind>();
            }
        }
    }
}