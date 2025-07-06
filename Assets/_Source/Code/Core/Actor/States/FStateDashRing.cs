using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateDashRing : FStateAirObject
    {
        private Vector3 _direction;
        
        public FStateDashRing(ActorBase owner) : base(owner) { }

        public override void OnExit()
        {
            base.OnExit();
            
            Model.StartAirRestore(0.5f);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Model.VelocityRotation(Kinematics.Velocity.normalized);
            
            CalculateTravelledDistance();

            if (Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down, 1f))
            {
                Kinematics.Normal = Vector3.up;
                StateMachine.SetState<FStateGround>();
                Model.StopAirRestore();
            }
        }
        
        public void SetDashRingDirection(Vector3 dir) => _direction = dir;
    }
}