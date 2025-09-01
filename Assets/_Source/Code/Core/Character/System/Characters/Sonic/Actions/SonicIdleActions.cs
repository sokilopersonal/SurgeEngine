using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.StateMachine;

namespace SurgeEngine._Source.Code.Core.Character.System.Characters.Sonic.Actions
{
    public class SonicIdleActions : CharacterActions
    {
        public SonicIdleActions(CharacterBase character) : base(character) { }

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
                    qs.SetDirection(QuickstepDirection.Left).SetRun(false);
                    StateMachine.SetState<FStateQuickstep>();
                }
                else if (Input.RightBumperPressed)
                {
                    var qs = StateMachine.GetState<FStateQuickstep>();
                    qs.SetDirection(QuickstepDirection.Right).SetRun(false);
                    StateMachine.SetState<FStateQuickstep>();
                }
            }
        }
    }
}