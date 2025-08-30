using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Gameplay.Inputs;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic.Actions
{
    public class SonicGroundActions : CharacterActions
    {
        private SlideConfig _slideConfig;
        private QuickStepConfig _quickstepConfig;

        public SonicGroundActions(CharacterBase character) : base(character) { }

        protected override void Connect(FStateMachine stateMachine)
        {
            stateMachine.GetState<FStateGround>().SetActions(this);
            
            Character.TryGetConfig(out _slideConfig);
            Character.TryGetConfig(out _quickstepConfig);
        }

        public override void Execute()
        {
            base.Execute();
            
            if (!Flags.HasFlag(FlagType.OutOfControl))
            {
                if (!SonicTools.IsBoost())
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