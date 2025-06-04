using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateRunQuickstep : FActorState, IStateTimeout
    {
        private QuickstepDirection _direction;
        private float _timer;
        
        private readonly QuickStepConfig _config;
        
        public FStateRunQuickstep(ActorBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
            Timeout = _config.runDelay;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _timer += dt / _config.runDuration;
            
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

        public void SetDirection(QuickstepDirection direction)
        {
            _direction = direction;
        }

        public QuickstepDirection GetDirection()
        {
            return _direction;
        }

        public float Timeout { get; set; }
    }
    
    public enum QuickstepDirection
    {
        Left = -1,
        Right = 1
    }
}