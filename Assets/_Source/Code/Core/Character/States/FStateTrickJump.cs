using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateTrickJump : FStateObject
    {
        public FStateTrickJump(CharacterBase owner) : base(owner) { }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Kinematics.ApplyGravity(Kinematics.Gravity);

            if (!Ignore)
            {
                if (Kinematics.CheckForGroundWithDirection(out _, Vector3.down))
                {
                    Kinematics.Normal = Vector3.up;
                    StateMachine.SetState<FStateGround>();
                }
            }
        }
    }
}