using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateSpring : FStateAirObject
    {
        public FStateSpring(ActorBase owner) : base(owner) { }

        public override void OnExit()
        {
            base.OnExit();
            
            Model.StartAirRestore(0.5f);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Model.VelocityRotation(Rigidbody.linearVelocity.normalized);
            
            CalculateTravelledDistance();
        }
    }
}