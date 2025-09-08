using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateDashRing : FStateAirObject
    {
        public FStateDashRing(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Model.StartAirRestore(0.3f);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Model.VelocityRotation(Kinematics.Velocity.normalized);
            
            CalculateTravelledDistance();

            if (Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down, castDistance: character.Config.castDistance * 0.6f))
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