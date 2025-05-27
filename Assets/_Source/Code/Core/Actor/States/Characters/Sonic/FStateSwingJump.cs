using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateSwingJump : FStateAir
    {
        public float failVel;
        public float successVel;
        protected float _maxAirTime;
        public FStateSwingJump(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _maxAirTime = 0.4f;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (GetAirTime() > _maxAirTime)
                StateMachine.SetState<FStateAir>();
        }

        public void Launch(bool successful)
        {
            _rigidbody.linearVelocity = (Actor.transform.forward + Actor.transform.up).normalized * (successful ? successVel : failVel);
            //Actor.effects.CreateLocus();
        }
    }
}
