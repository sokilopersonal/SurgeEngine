using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Misc;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.ActorStates.SonicSubStates
{
    public class FSweepKick : FActorSubState
    {
        private ISweepKickHandler _sweepKickHandler;
        private int pressAmount;
        private float lastPressTime;
        public FSweepKick(Actor owner) : base(owner)
        {
            actor.input.OnButtonPressed += ButtonPressed;
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is ISweepKickHandler casted)
            {
                _sweepKickHandler = casted;
            }
            else
            {
                _sweepKickHandler = null;
                pressAmount = 0;
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
           
            if (Time.time - lastPressTime > 0.3f)
                pressAmount = 0;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
        }

        private void ButtonPressed(ButtonType button)
        {
            if (button != ButtonType.B || _sweepKickHandler == null)
                return;

            pressAmount += 1;
            if (pressAmount == 2 && Time.time - lastPressTime <= 0.3f)
            {
                actor.stateMachine.SetState<FStateSweepKick>();
                pressAmount = 0;
            }
            lastPressTime = Time.time;
        }
    }
}