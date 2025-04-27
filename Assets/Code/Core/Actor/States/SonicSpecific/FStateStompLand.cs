using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.SonicSpecific
{
    public class FStateStompLand : FStateMove
    {
        private float _timer;
        private const float WaitTime = 0.15f;
        
        public FStateStompLand(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
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
            bool ceiling = Common.CheckForCeiling(out RaycastHit data);
            if (Common.TickTimer(ref _timer, WaitTime))
            {
                if (Input.BHeld)
                {
                    StateMachine.SetState<FStateSit>();
                }
                else
                {
                    if (!ceiling)
                        StateMachine.SetState<FStateIdle>();
                    else
                        StateMachine.SetState<FStateSit>();
                }
            }
        }
    }
}