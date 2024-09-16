using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public abstract class FStateMove : FActorState
    {
        protected Rigidbody _rigidbody { get; private set; }
        
        public override void OnEnter()
        {
            if (_rigidbody == null)
            {
                _rigidbody = actor.GetComponent<Rigidbody>();
            }
        }
    }
}