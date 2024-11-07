using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateGrindJump : FStateJump
    {
        public FStateGrindJump(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _maxAirTime = 0.3f;
        }
    }
}