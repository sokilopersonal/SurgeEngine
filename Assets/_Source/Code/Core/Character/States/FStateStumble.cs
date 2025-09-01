using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateStumble : FCharacterState
    {
        private float _time;
        private float _ignoreTime;
        
        public FStateStumble(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();

            _time = 0;
            _ignoreTime = 0.1f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Utility.TickTimer(ref _time, _time, false))
            {
                StateMachine.SetState<FStateGround>();
            }
            
            Utility.TickTimer(ref _ignoreTime, _ignoreTime, false);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Kinematics.CheckForGround(out RaycastHit hit))
            {
                if (_ignoreTime <= 0)
                {
                    Kinematics.Normal = Vector3.up;
                    Kinematics.Snap(hit.point, Vector3.up);
                    
                    Kinematics.Project(hit.normal);
                }
            }
            
            Kinematics.ApplyGravity(Kinematics.Gravity);
        }

        public void SetNoControlTime(float time) => _time = Mathf.Clamp(time, 0, time);
    }
}