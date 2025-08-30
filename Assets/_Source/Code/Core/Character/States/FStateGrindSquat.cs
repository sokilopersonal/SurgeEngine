using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Core.Actor.States
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