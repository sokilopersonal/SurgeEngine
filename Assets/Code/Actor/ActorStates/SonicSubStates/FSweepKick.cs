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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.ActorStates.SonicSubStates
{
    public class FSweepKick : FActorSubState
    {
        private float lastClickTime;

        private readonly SweepConfig _config;
        private ISweepKickHandler _handler;

        public FSweepKick(Actor owner) : base(owner)
        {
            _config = (owner as Sonic).sweepKickConfig;
            
            actor.input.OnButtonPressed += ButtonPressed;
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is ISweepKickHandler casted)
            {
                _handler = casted;
            }
            else
            {
                _handler = null;
            }
        }

        private void ButtonPressed(ButtonType button)
        {
            if (button != ButtonType.B || _handler == null)
                return;
            float currentTime = Time.time;
            if (currentTime - lastClickTime < _config.inputBufferTime)
            {
                actor.stateMachine.SetState<FStateSweepKick>();
            }
            lastClickTime = currentTime;
        }
    }
    
    public interface ISweepKickHandler { }
}