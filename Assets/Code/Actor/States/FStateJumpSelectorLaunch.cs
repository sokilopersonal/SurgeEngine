using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States
{
    public class FStateJumpSelectorLaunch : FStateMove
    {
        private float _timer;
        
        public FStateJumpSelectorLaunch(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (_timer > 0)
            {
                _timer -= dt;
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public void SetData(Vector3 force, float keepVelocityTime)
        {
            _rigidbody.linearVelocity = force;
            _timer = keepVelocityTime;
        }
    }
}