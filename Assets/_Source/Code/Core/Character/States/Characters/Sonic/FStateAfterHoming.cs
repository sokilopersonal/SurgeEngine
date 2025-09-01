using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic
{
    public class FStateAfterHoming : FCharacterState
    {
        private readonly HomingConfig _config;
        
        public FStateAfterHoming(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.ResetVelocity();
            Kinematics.Rigidbody.linearVelocity += Vector3.up * _config.afterForce;
            
            Vector3 currentRotation = Kinematics.Rigidbody.rotation.eulerAngles;
            Vector3 newRotation = new Vector3(0f, currentRotation.y, 0f);
            Kinematics.Rigidbody.rotation = Quaternion.Euler(newRotation);
            
            StateMachine.SetState<FStateAir>();
        }
    }
}