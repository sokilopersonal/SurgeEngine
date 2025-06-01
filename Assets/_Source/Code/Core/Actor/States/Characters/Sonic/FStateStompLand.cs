using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateStompLand : FActorState
    {
        private float _timer;
        private const float WaitTime = 0.15f;
        
        public FStateStompLand(ActorBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _timer = WaitTime;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            bool ceiling = Kinematics.CheckForCeiling(out RaycastHit data);
            if (Utility.TickTimer(ref _timer, WaitTime))
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

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Kinematics.CheckForGround(out RaycastHit hit))
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;

                Kinematics.Snap(Kinematics.Point, Vector3.up, true);
            }
        }
    }
}