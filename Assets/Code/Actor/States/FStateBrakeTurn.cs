using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States
{
    public class FStateBrakeTurn : FStateMove, IDamageableState
    {
        private float _timer;
        private Quaternion _rigidbodyRotation;
        
        public FStateBrakeTurn(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
            _rigidbodyRotation = _rigidbody.rotation;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            float duration = 0.3f;

            if (_timer < duration)
            {
                _timer += dt;
                
                _rigidbody.rotation = Quaternion.Lerp(_rigidbodyRotation, _rigidbodyRotation * Quaternion.Euler(0f, 180f, 0f), _timer / duration);
                Model.root.rotation = _rigidbody.rotation;
            }
            else
            {
                StateMachine.SetState<FStateIdle>();
            }
        }
    }
}