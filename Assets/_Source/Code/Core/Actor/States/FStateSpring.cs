using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateSpring : FStateAirObject
    {
        private float _keepVelocityDistance;
        
        private float _travelledDistance;
        private Vector3 _direction;
        
        public FStateSpring(ActorBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.SetDetachTime(0.1f);
            _travelledDistance = 0;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Model.StartAirRestore(0.5f);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Model.VelocityRotation(_direction);
            _travelledDistance += Kinematics.Speed * dt;

            if (_travelledDistance > _keepVelocityDistance)
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public void SetKeepVelocityDistance(float distance) => _keepVelocityDistance = distance;
        public void SetSpringDirection(Vector3 dir) => _direction = dir;
    }
}