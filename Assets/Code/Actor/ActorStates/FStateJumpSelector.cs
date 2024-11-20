using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateJumpSelector : FStateMove
    {
        public FStateJumpSelector(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
}