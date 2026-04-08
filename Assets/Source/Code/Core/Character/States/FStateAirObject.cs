using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public abstract class FStateAirObject : FStateObject, IWallJumpDetect, ISkip2D
    {
        public bool WallDetected { get; set; }
        
        protected float KeepVelocityDistance;
        protected float TravelledDistance;

        protected FStateAirObject(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            TravelledDistance = 0;
        }

        protected void CalculateTravelledDistance()
        {
            TravelledDistance += Kinematics.Speed * Time.fixedDeltaTime;
            if (TravelledDistance > KeepVelocityDistance + 0.5f && !WallDetected)
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public void SetKeepVelocityDistance(float distance) => KeepVelocityDistance = distance;
    }
}