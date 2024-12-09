using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateAfterHoming : FActorState
    {
        private float _timer;
        private HomingConfig _config;
        
        public FStateAfterHoming(Actor owner) : base(owner)
        {
            _config = (owner as Sonic).homingConfig;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            _timer = _config.delay;
            
            Common.ResetVelocity(ResetVelocityType.Both);
            Kinematics.Rigidbody.linearVelocity += Vector3.up * _config.afterForce;
            
            Actor.transform.localRotation = Quaternion.Euler(0f, Actor.model.transform.localRotation.eulerAngles.y, 0f);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (_timer > 0)
            {
                _timer -= dt;
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}