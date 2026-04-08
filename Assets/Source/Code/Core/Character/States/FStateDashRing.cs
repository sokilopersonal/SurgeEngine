using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateDashRing : FStateAirObject
    {
        public Vector3 Origin { get; set; }
        public Vector3 Direction { get; set; }
        
        private Vector3 _snapVelocity;
        
        public FStateDashRing(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();

            _snapVelocity = Vector3.zero;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Model.StartAirRestore(0.3f);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            ApplyLateralSnapping(
                Origin, 
                Direction, 
                ref _snapVelocity, 
                0.1f
            );
            
            Model.VelocityRotation(Kinematics.Velocity.normalized);
            
            CalculateTravelledDistance();

            if (Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down, castDistance: Character.Config.castDistance * 0.6f))
            {
                Kinematics.Normal = Vector3.up;
                Rigidbody.rotation = Quaternion.FromToRotation(Rigidbody.transform.up, Vector3.up) * Rigidbody.rotation;
                Rigidbody.linearVelocity = Quaternion.FromToRotation(Rigidbody.transform.up, Vector3.up) * Kinematics.Velocity;
                StateMachine.SetState<FStateGround>();
                Model.StopAirRestore();
            }
        }
    }
}