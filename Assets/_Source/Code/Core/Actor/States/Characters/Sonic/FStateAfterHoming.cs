using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateAfterHoming : FActorState
    {
        private readonly HomingConfig _config;
        
        public FStateAfterHoming(ActorBase owner) : base(owner)
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