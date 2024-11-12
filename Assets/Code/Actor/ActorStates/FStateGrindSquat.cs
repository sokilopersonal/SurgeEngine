using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateGrindSquat : FStateGrind
    {
        public FStateGrindSquat(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _grindGravityPower = 13;
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