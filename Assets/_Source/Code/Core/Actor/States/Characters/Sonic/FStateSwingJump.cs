using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateSwingJump : FStateAir
    {
        public float failVel;
        public float successVel;
        
        public FStateSwingJump(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (GetAirTime() > 0.4f)
                StateMachine.SetState<FStateAir>();
        }

        public void Launch(bool successful)
        {
            _rigidbody.linearVelocity = (Actor.transform.forward + Actor.transform.up).normalized * (successful ? successVel : failVel);
        }
    }
}
