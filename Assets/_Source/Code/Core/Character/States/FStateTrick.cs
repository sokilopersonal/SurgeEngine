using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateTrick : FCharacterState
    {
        public FStateTrick(CharacterBase owner) : base(owner) { }
        
        private float _timer;
        private float _outOfControl;

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Utility.TickTimer(ref _timer, _outOfControl, false))
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            Kinematics.ApplyGravity(Kinematics.Gravity);
            
            if (Kinematics.CheckForGround(out _))
            {
                StateMachine.SetState<FStateGround>();
            }
        }

        public void SetTimer(float time)
        {
            _timer = time;
            _outOfControl = time;
        }
    }
}