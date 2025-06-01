using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic.Actions
{
    public class SonicIdleActions : ActorActions
    {
        protected override void Connect(FStateMachine stateMachine)
        {
            stateMachine.GetState<FStateIdle>().SetActions(this);
        }

        public override void Execute()
        {
            base.Execute();
            
            if (!Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.BPressed)
                {
                    StateMachine.SetState<FStateSit>();
                }
                
                if (Input.LeftBumperPressed)
                {
                    var qs = StateMachine.GetState<FStateQuickstep>();
                    qs.SetDirection(QuickstepDirection.Left);
                    StateMachine.SetState<FStateQuickstep>();
                }
                else if (Input.RightBumperPressed)
                {
                    var qs = StateMachine.GetState<FStateQuickstep>();
                    qs.SetDirection(QuickstepDirection.Right);
                    StateMachine.SetState<FStateQuickstep>();
                }
            }
        }
    }
}