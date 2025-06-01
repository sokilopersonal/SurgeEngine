using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateSwingJump : FStateAir
    {
        public float failVel;
        public float successVel;
        
        public FStateSwingJump(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Kinematics.AirTime > 0.4f)
                StateMachine.SetState<FStateAir>();
        }

        public void Launch(bool successful)
        {
            Rigidbody.linearVelocity = (Actor.transform.forward + Actor.transform.up).normalized * (successful ? successVel : failVel);
        }
    }
}
