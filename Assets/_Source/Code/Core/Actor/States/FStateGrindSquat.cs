using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateGrindSquat : FStateGrind
    {
        public FStateGrindSquat(ActorBase owner) : base(owner)
        {
            _grindGravityPower = 14;
        }

        public override void OnEnter()
        {
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Input.DownHeld)
            {
                StateMachine.SetState<FStateGrind>();
            }
        }
    }
}