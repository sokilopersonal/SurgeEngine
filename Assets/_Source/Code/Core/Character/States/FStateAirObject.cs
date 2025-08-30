using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public abstract class FStateAirObject : FStateObject
    {
        protected float _keepVelocityDistance;
        protected float _travelledDistance;
        
        protected FStateAirObject(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _travelledDistance = 0;
        }

        protected void CalculateTravelledDistance()
        {
            _travelledDistance += Kinematics.Speed * Time.deltaTime;
            if (_travelledDistance > _keepVelocityDistance)
            {
                StateMachine.SetState<FStateAir>();
            }
        }
        
        public void SetKeepVelocityDistance(float distance) => _keepVelocityDistance = distance;
    }
}