using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.StateMachine;
using SurgeEngine._Source.Code.Gameplay.CommonObjects;

namespace SurgeEngine._Source.Code.Core.Character.System.Characters.Sonic.Actions
{
    public class SonicAirActions : CharacterActions
    {
        public SonicAirActions(CharacterBase character) : base(character) { }

        protected override void Connect(FStateMachine stateMachine)
        {
            foreach (var airState in stateMachine.GetAllStatesOfType<FStateAir>())
            {
                airState.SetActions(this);
            }
        }

        public override void Execute()
        {
            base.Execute();
            
            if (Kinematics.AirTime > 0.1f)
            {
                if (!Flags.HasFlag(FlagType.OutOfControl))
                {
                    HomingTarget homingTarget = (Kinematics as SonicKinematics)?.HomingTarget;

                    if (Input.APressed)
                    {
                        if (StateMachine.PreviousState is not FStateHoming or FStateAirBoost)
                        {
                            if (homingTarget != null)
                            {
                                StateMachine.SetState<FStateHoming>()?.SetTarget(homingTarget);
                            }
                            else
                            {
                                StateMachine.SetState<FStateHoming>();
                            }
                        }
                    }
                }
            }

            if (!Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.BPressed)
                {
                    StateMachine.SetState<FStateStomp>();
                }
            }
        }
    }
}