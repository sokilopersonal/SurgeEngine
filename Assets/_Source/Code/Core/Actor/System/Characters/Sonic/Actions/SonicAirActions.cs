using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Gameplay.CommonObjects;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic.Actions
{
    public class SonicAirActions : ActorActions
    {
        public SonicAirActions(ActorBase actor) : base(actor) { }

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