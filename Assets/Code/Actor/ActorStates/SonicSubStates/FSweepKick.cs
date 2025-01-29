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
        private List<string> includedAnimationStates = new List<string>()
        {
            "SitEnter",
            "SitExit",
            "Sliding",
            "SlideToSit",
            "CrawlEnter",
            "CrawlExit"
        };
        public FSweepKick(Actor owner) : base(owner)
        {
            actor.input.OnButtonPressed += ButtonPressed;
        }

        private void ButtonPressed(ButtonType button)
        {
            if (button != ButtonType.B || !includedAnimationStates.Contains(actor.animation.GetCurrentAnimationState()))
                return;
            actor.stateMachine.SetState<FStateSweepKick>();
        }
    }
}