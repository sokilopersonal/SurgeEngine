using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateJumpPanel : FStateObject
    {
        private bool _isDelux;
        public bool IsDelux => _isDelux;
        
        private float _outOfControlTimer;
        
        public FStateJumpPanel(CharacterBase owner) : base(owner) { }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _outOfControlTimer -= dt;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Kinematics.ApplyGravity(Kinematics.Gravity);

            if (!Ignore)
            {
                if (Kinematics.CheckForGroundWithDirection(out _, Vector3.down, castDistance: character.Config.castDistance * 0.5f))
                {
                    StateMachine.SetState<FStateGround>();
                }
                
                if (_outOfControlTimer <= 0)
                {
                    StateMachine.SetState<FStateAir>();
                }
            }
        }

        public void SetDelux(bool isDelux) => _isDelux = isDelux;

        public void SetKeepVelocity(float outOfControl)
        {
            _outOfControlTimer = outOfControl;
        }
    }
}