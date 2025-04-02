using SurgeEngine.Code.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States
{
    public class FStateGrindJump : FStateJump
    {
        public FStateGrindJump(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _maxAirTime = 0.3f;
        }
    }
}