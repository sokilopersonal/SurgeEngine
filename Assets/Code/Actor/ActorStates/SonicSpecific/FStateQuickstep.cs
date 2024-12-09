using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateQuickstep : FStateMove
    {
        private int _direction = 0;
        
        public FStateQuickstep(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }
        
        public void SetDirection(int direction) => _direction = direction;
    }
}