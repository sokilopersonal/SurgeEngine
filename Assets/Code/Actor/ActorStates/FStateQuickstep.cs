using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
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