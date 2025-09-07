using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
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
            if (_travelledDistance > _keepVelocityDistance + 0.25f)
            {
                StateMachine.SetState<FStateAir>();
            }
        }
        
        public void SetKeepVelocityDistance(float distance) => _keepVelocityDistance = distance;
    }
}