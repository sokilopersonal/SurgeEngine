using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateJumpSelectorLaunch : FActorState
    {
        private float _timer;
        
        public FStateJumpSelectorLaunch(ActorBase owner) : base(owner)
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
            Rigidbody.linearVelocity = force;
            _timer = keepVelocityTime;
        }
    }
}