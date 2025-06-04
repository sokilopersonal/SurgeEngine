using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateQuickstep : FActorState, IStateTimeout
    {
        public float Timeout { get; set; }
        
        private QuickstepDirection _direction;
        private float _timer;

        private readonly QuickStepConfig _config;

        public FStateQuickstep(ActorBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Timeout = _config.delay;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _timer += dt / _config.duration;

            if (_timer >= 1f)
            {
                StateMachine.SetState<FStateGround>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (!Kinematics.CheckForGround(out _))
            {
                Actor.StateMachine.SetState<FStateAir>();
            }
        }

        public void SetDirection(QuickstepDirection direction) => _direction = direction;

        public QuickstepDirection GetDirection()
        {
            return _direction;
        }
    }
}