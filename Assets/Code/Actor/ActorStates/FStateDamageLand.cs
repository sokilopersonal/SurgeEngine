using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateDamageLand : FStateMove
    {
        private float _timer;
        
        public FStateDamageLand(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 2f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            Common.ResetVelocity(ResetVelocityType.Both);
            
            if (Common.CheckForGround(out var hit))
            {
                Kinematics.Snap(hit.point, hit.normal, true);
            }
            
            if (Common.TickTimer(ref _timer, 2f, false))
            {
                StateMachine.SetState<FStateIdle>();
            }
        }
    }
}