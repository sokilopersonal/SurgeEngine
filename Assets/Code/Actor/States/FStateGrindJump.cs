using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateGrindJump : FStateJump
    {
        public FStateGrindJump(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _maxAirTime = 0.3f;
        }
    }
}