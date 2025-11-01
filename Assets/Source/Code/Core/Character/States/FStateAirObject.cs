using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public abstract class FStateAirObject : FStateObject
    {
        protected float keepVelocityDistance;
        protected float travelledDistance;
        
        protected FStateAirObject(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            travelledDistance = 0;
        }

        protected void CalculateTravelledDistance()
        {
            travelledDistance += Kinematics.Speed * Time.fixedDeltaTime;
            if (travelledDistance > keepVelocityDistance + 0.5f)
            {
                StateMachine.SetState<FStateAir>();
            }
        }
        
        public void SetKeepVelocityDistance(float distance) => keepVelocityDistance = distance;
    }
}