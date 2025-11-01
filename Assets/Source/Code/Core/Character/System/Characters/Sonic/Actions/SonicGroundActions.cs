using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic.Actions
{
    public class SonicGroundActions : CharacterActions
    {
        private SlideConfig _slideConfig;
        private QuickStepConfig _quickstepConfig;

        private FBoost _boost;

        public SonicGroundActions(CharacterBase character) : base(character) { }

        protected override void Connect(FStateMachine stateMachine)
        {
            stateMachine.GetState<FStateGround>().SetActions(this);
            
            Character.TryGetConfig(out _slideConfig);
            Character.TryGetConfig(out _quickstepConfig);
            
            Character.StateMachine.GetState(out _boost);
        }

        public override void Execute()
        {
            base.Execute();
            
            if (!Flags.HasFlag(FlagType.OutOfControl))
            {
                if (_boost != null && _boost.Active)
                {
                    if (Kinematics.Skidding && Kinematics.Speed > 15f)
                    {
                        StateMachine.SetState<FStateBrake>();
                    }
                }

                float minSpeed = _slideConfig.minSpeed;
                minSpeed += minSpeed * 1.5f;
                float dot = Kinematics.MoveDot;
                float abs = Mathf.Abs(dot);
            
                bool readyForDrift = Kinematics.Speed > 5f && abs < 0.4f && !Mathf.Approximately(dot, 0f);
                bool readyForSlide = Kinematics.Speed > minSpeed;

                if (_quickstepConfig)
                {
                    if (Input.LeftBumperPressed)
                    {
                        StateMachine.GetState<FStateQuickstep>().SetDirection(QuickstepDirection.Left).SetRun(true);
                        StateMachine.SetState<FStateQuickstep>();
                    }
                    else if (Input.RightBumperPressed)
                    {
                        StateMachine.GetState<FStateQuickstep>().SetDirection(QuickstepDirection.Right).SetRun(true);
                        StateMachine.SetState<FStateQuickstep>();
                    }
                }
                
                if (Input.BHeld)
                {
                    if (readyForSlide && !readyForDrift)
                    {
                        StateMachine.SetState<FStateSlide>();
                    }
                    else if(!readyForSlide && !readyForDrift)
                    {
                        StateMachine.SetState<FStateCrawl>();
                    }

                    if (readyForDrift)
                    {
                        StateMachine.SetState<FStateDrift>();
                    }
                }

                if (((SonicInput)Input).DriftHeld)
                {
                    if (readyForDrift)
                    {
                        StateMachine.SetState<FStateDrift>();
                    }
                }
            }
        }
    }
}