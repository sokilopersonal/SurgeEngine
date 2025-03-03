using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateStompLand : FStateMove
    {
        private float _timer;
        private const float WaitTime = 0.15f;
        
        public FStateStompLand(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _timer = WaitTime;
            
            Common.ResetVelocity(ResetVelocityType.Both);
            if (Common.CheckForGround(out RaycastHit hit))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;

                Kinematics.Snap(point, normal, true);
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Common.TickTimer(ref _timer, WaitTime))
            {
                if (Input.BHeld)
                {
                    StateMachine.SetState<FStateSit>();
                }
                else
                {
                    StateMachine.SetState<FStateIdle>();
                }
            }
        }
    }
}