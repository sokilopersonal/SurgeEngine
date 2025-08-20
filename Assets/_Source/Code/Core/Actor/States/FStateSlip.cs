using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateSlip : FCharacterState
    {
        private float _timer;
        
        public FStateSlip(CharacterBase owner) : base(owner) { }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            Utility.TickTimer(ref _timer, _timer, false);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Kinematics.Normal = Vector3.up;
            bool ground = Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down);
            bool predictedGround = Kinematics.CheckForPredictedGround(dt, character.Config.castDistance, 4);
            if (ground)
            {
                if (!predictedGround)
                {
                    Rigidbody.linearVelocity += Vector3.down * (Kinematics.Gravity * dt);
                    _timer = 0.15f;
                }
                else
                {
                    if (_timer <= 0)
                    {
                        StateMachine.SetState<FStateGround>();
                    }
                }
                
                Kinematics.Snap(hit.point, Vector3.up);
                Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, hit.normal);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}