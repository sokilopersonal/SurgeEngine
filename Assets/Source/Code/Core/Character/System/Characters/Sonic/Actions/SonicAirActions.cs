using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Infrastructure.Tools.Managers;
using UnityEngine.TextCore.Text;
using Zenject;

namespace SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic.Actions
{
    public class SonicAirActions : CharacterActions
    {
        public SonicAirActions(CharacterBase character) : base(character) { }

        [Inject] private UserInput _userInput;

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
                    if (Character.TryGetComponent(out HomingTargetDetector detector) && Character.StateMachine.GetState(out FBoost boost))
                    {
                        bool homingX = Input.XPressed && _userInput.GetData().homingOnX.Value && (detector.Target != null || detector.Target == null && !boost.CanBoost());
                        bool homingA = Input.APressed && !_userInput.GetData().homingOnX.Value;
                        if (homingX || homingA)
                        {
                            if (StateMachine.PreviousState is not FStateHoming or FStateAirBoost)
                            {
                                var homingTarget = detector.Target;
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