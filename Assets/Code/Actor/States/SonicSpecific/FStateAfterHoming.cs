using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States.SonicSpecific
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
            
            Common.ResetVelocity(ResetVelocityType.Both);
            Kinematics.Rigidbody.linearVelocity += Vector3.up * _config.afterForce;
            
            Actor.transform.localRotation = Quaternion.Euler(0f, Actor.model.transform.localRotation.eulerAngles.y, 0f);
            
            StateMachine.SetState<FStateAir>();
        }
    }
}