using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateStumble : FCharacterState
    {
        private float _time;
        
        public FStateStumble(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.SetDetachTime(0.2f);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (_time > 0)
            {
                _time -= dt;
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Kinematics.CheckForGround(out RaycastHit hit))
            {
                if (Kinematics.GetAttachState())
                {
                    Kinematics.Normal = Vector3.up;
                    Kinematics.Snap(hit.point, Vector3.up);
                    
                    Kinematics.Project(hit.normal);

                    if (_time <= 0)
                    {
                        StateMachine.SetState<FStateGround>();
                    }
                }
            }
            
            Kinematics.ApplyGravity(Kinematics.Gravity);
        }

        public void SetNoControlTime(float time) => _time = Mathf.Clamp(time, 0, time);
    }
}